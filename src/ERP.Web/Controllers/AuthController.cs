using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Constants;
using ERP.Web.Services;
using ERP.Web.DTOs.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para autenticação
/// </summary>
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly TwoFactorService _twoFactorService;
    private readonly MasterDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="userManager">UserManager</param>
    /// <param name="signInManager">SignInManager</param>
    /// <param name="jwtService">JwtService</param>
    /// <param name="twoFactorService">TwoFactorService</param>
    /// <param name="dbContext">MasterDbContext</param>
    /// <param name="logger">Logger</param>
    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService,
        TwoFactorService twoFactorService,
        MasterDbContext dbContext,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _twoFactorService = twoFactorService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validar request
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return ValidationError(new Dictionary<string, List<string>>
                {
                    { "username", new List<string> { "Username is required" } },
                    { "password", new List<string> { "Password is required" } }
                });
            }

            // Buscar usuário
            var user = await _userManager.FindByNameAsync(request.Username) ??
                      await _userManager.FindByEmailAsync(request.Username);

            if (user == null)
            {
                // Logar tentativa de login
                await LogLoginAttemptAsync(null, request.Username, false, true);
                return Unauthorized(ErrorMessages.InvalidCredentials);
            }

            // Verificar senha
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                // Logar tentativa de login
                await LogLoginAttemptAsync(user, request.Username, false, wrongPassword: true);
                return Unauthorized(ErrorMessages.InvalidCredentials);
            }

            // Verificar se a conta está bloqueada
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                await LogLoginAttemptAsync(user, request.Username, false, accountLocked: true);
                return Unauthorized(ErrorMessages.AccountLocked);
            }

            // Verificar se a conta está ativa
            if (user.Status != UserStatus.Active)
            {
                await LogLoginAttemptAsync(user, request.Username, false);
                return Unauthorized(ErrorMessages.AccountInactive);
            }

            // Verificar se o 2FA está habilitado
            if (user.TwoFactorEnabled)
            {
                // Gerar token temporário para 2FA
                var tempToken = await _jwtService.GenerateTokenAsync(
                    user,
                    user.TenantId,
                    await _userManager.GetRolesAsync(user));

                return Success(new LoginResponse
                {
                    RequiresTwoFactor = true,
                    TwoFactorToken = tempToken,
                    UserId = user.Id
                }, "2FA required");
            }

            // Gerar token JWT
            var token = await _jwtService.GenerateTokenAsync(
                user,
                user.TenantId,
                await _userManager.GetRolesAsync(user));

            // Gerar refresh token
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                user,
                GetClientIp(),
                Request.Headers["User-Agent"]);

            // Atualizar última data de login
            user.LastLogin = DateTime.UtcNow;
            user.LastLoginIp = GetClientIp();
            user.FailedLoginAttempts = 0;
            await _userManager.UpdateAsync(user);

            // Logar login bem-sucedido
            await LogLoginAttemptAsync(user, request.Username, true);

            return Success(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hora
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                }
            }, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Login com 2FA
    /// </summary>
    /// <param name="request">Dados de login com 2FA</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login-2fa")]
    public async Task<IActionResult> LoginWithTwoFactor([FromBody] LoginTwoFactorRequest request)
    {
        try
        {
            // Validar request
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Code))
            {
                return ValidationError(new Dictionary<string, List<string>>
                {
                    { "userId", new List<string> { "User ID is required" } },
                    { "code", new List<string> { "2FA code is required" } }
                });
            }

            // Obter usuário
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            // Verificar código 2FA
            var isValid = await _twoFactorService.ValidateUserCodeAsync(user, request.Code);

            if (!isValid)
            {
                // Verificar código de recuperação
                isValid = await _twoFactorService.ValidateRecoveryCodeAsync(user, request.Code);

                if (!isValid)
                {
                    await LogLoginAttemptAsync(user, user.UserName, false, twoFactorFailed: true);
                    return Unauthorized(ErrorMessages.InvalidTwoFactorCode);
                }
            }

            // Gerar token JWT
            var token = await _jwtService.GenerateTokenAsync(
                user,
                user.TenantId,
                await _userManager.GetRolesAsync(user));

            // Gerar refresh token
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                user,
                GetClientIp(),
                Request.Headers["User-Agent"]);

            // Atualizar última data de login
            user.LastLogin = DateTime.UtcNow;
            user.LastLoginIp = GetClientIp();
            user.FailedLoginAttempts = 0;
            await _userManager.UpdateAsync(user);

            // Logar login bem-sucedido
            await LogLoginAttemptAsync(user, user.UserName, true);

            return Success(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                }
            }, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during 2FA login for user {UserId}", request.UserId);
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="request">Dados de refresh token</param>
    /// <returns>Novo token JWT</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validar request
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return ValidationError(new Dictionary<string, List<string>>
                {
                    { "token", new List<string> { "Token is required" } },
                    { "refreshToken", new List<string> { "Refresh token is required" } }
                });
            }

            // Validar token JWT
            var principal = await _jwtService.ValidateTokenAsync(request.Token);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !Guid.TryParse(userId, out var userIdGuid))
            {
                return Unauthorized(ErrorMessages.InvalidToken);
            }

            // Obter usuário
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            // Validar refresh token
            var isValidRefreshToken = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, user);

            if (!isValidRefreshToken)
            {
                return Unauthorized(ErrorMessages.InvalidRefreshToken);
            }

            // Revogar refresh token antigo
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, user, "Refreshed");

            // Gerar novo token JWT
            var newToken = await _jwtService.GenerateTokenAsync(
                user,
                user.TenantId,
                await _userManager.GetRolesAsync(user));

            // Gerar novo refresh token
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(
                user,
                GetClientIp(),
                Request.Headers["User-Agent"]);

            return Success(new RefreshTokenResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600
            }, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Error(ErrorMessages.InvalidToken);
        }
    }

    /// <summary>
    /// Logout
    /// </summary>
    /// <returns>Sucesso</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                // Revogar todos os refresh tokens
                await _jwtService.RevokeAllRefreshTokensAsync(user, "Logged out");

                // Sign out
                await _signInManager.SignOutAsync();
            }

            return Success("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", GetCurrentUserId());
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Obtém o usuário atual
    /// </summary>
    /// <returns>Dados do usuário</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            return Success(new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                EmailVerified = user.EmailVerified,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                Locale = user.Locale,
                Timezone = user.Timezone,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user for {UserId}", GetCurrentUserId());
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Habilita 2FA
    /// </summary>
    /// <param name="request">Dados para habilitar 2FA</param>
    /// <returns>Sucesso</returns>
    [HttpPost("enable-2fa")]
    [Authorize]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            // Verificar código
            var isValid = _twoFactorService.ValidateCode(request.Secret, request.Code);

            if (!isValid)
            {
                return ValidationError(new Dictionary<string, List<string>>
                {
                    { "code", new List<string> { "Invalid 2FA code" } }
                });
            }

            // Salvar segredo e habilitar 2FA
            user.TwoFactorSecret = request.Secret;
            user.TwoFactorEnabled = true;
            user.TwoFactorRecoveryCodes = string.Join(",", _twoFactorService.GenerateRecoveryCodes());

            await _userManager.UpdateAsync(user);

            return Success(new TwoFactorResponse
            {
                Enabled = true,
                RecoveryCodes = user.TwoFactorRecoveryCodes?.Split(',')?.ToList()
            }, "2FA enabled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling 2FA for user {UserId}", GetCurrentUserId());
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Desabilita 2FA
    /// </summary>
    /// <param name="request">Dados para desabilitar 2FA</param>
    /// <returns>Sucesso</returns>
    [HttpPost("disable-2fa")]
    [Authorize]
    public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            // Verificar senha
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(ErrorMessages.CurrentPasswordIncorrect);
            }

            // Desabilitar 2FA
            user.TwoFactorEnabled = false;
            user.TwoFactorSecret = null;
            user.TwoFactorRecoveryCodes = null;

            await _userManager.UpdateAsync(user);

            return Success("2FA disabled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA for user {UserId}", GetCurrentUserId());
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Gera segredo para 2FA
    /// </summary>
    /// <returns>Segredo e URI para QR Code</returns>
    [HttpGet("2fa-secret")]
    [Authorize]
    public async Task<IActionResult> GenerateTwoFactorSecret()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Unauthorized(ErrorMessages.UserNotFound);
            }

            // Gerar segredo
            var (secret, qrCodeUri) = _twoFactorService.GenerateSecret(user);

            return Success(new TwoFactorSecretResponse
            {
                Secret = secret,
                QrCodeUri = qrCodeUri
            }, "2FA secret generated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating 2FA secret for user {UserId}", GetCurrentUserId());
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Obtém o IP do cliente
    /// </summary>
    /// <returns>IP do cliente</returns>
    private string GetClientIp()
    {
        return Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
               Request.HttpContext.Connection.RemoteIpAddress?.ToString() ??
               "unknown";
    }

    /// <summary>
    /// Loga uma tentativa de login
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="username">Username</param>
    /// <param name="isSuccess">Sucesso</param>
    /// <param name="wrongPassword">Senha errada</param>
    /// <param name="userNotFound">Usuário não encontrado</param>
    /// <param name="accountLocked">Conta bloqueada</param>
    /// <param name="twoFactorFailed">2FA falhou</param>
    /// <returns>Task</returns>
    private async Task LogLoginAttemptAsync(
        User? user,
        string username,
        bool isSuccess,
        bool wrongPassword = false,
        bool userNotFound = false,
        bool accountLocked = false,
        bool twoFactorFailed = false)
    {
        try
        {
            var loginAttempt = new LoginAttempt
            {
                UserId = user?.Id,
                Username = username,
                ClientIp = GetClientIp(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IsSuccess = isSuccess,
                WrongPassword = wrongPassword,
                UserNotFound = userNotFound,
                AccountLocked = accountLocked,
                TwoFactorFailed = twoFactorFailed,
                AttemptTime = DateTime.UtcNow,
                TenantId = user?.TenantId
            };

            await _dbContext.LoginAttempts.AddAsync(loginAttempt);
            await _dbContext.SaveChangesAsync();

            // Atualizar tentativas falhas
            if (user != null && !isSuccess)
            {
                user.FailedLoginAttempts++;
                await _userManager.UpdateAsync(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging login attempt for user {Username}", username);
        }
    }
}
