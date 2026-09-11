using ERP.Master.Data;
using ERP.Master.Models;
using ERP.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP.Shared.Services;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtService _jwtService;
    private readonly TwoFactorService _twoFactorService;
    private readonly MasterDbContext _masterDbContext;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        JwtService jwtService,
        TwoFactorService twoFactorService,
        MasterDbContext masterDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _twoFactorService = twoFactorService;
        _masterDbContext = masterDbContext;
    }

    /// <summary>
    /// Realiza login do usuário e retorna tokens JWT
    /// </summary>
    public async Task<AuthResult> LoginAsync(string email, string password, string ip, string userAgent)
    {
        // 1. Encontrar usuário
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        // 2. Verificar se conta está bloqueada
        if (await _userManager.IsLockedOutAsync(user))
        {
            await RegisterLoginAttempt(user.Id, email, ip, userAgent, false, false, false, true);
            throw new UnauthorizedAccessException("Conta bloqueada por muitas tentativas falhas. Tente novamente mais tarde.");
        }

        // 3. Verificar se usuário está ativo
        if (!user.IsActive)
        {
            await RegisterLoginAttempt(user.Id, email, ip, userAgent, false, false, false, false);
            throw new UnauthorizedAccessException("Usuário inativo. Contate o administrador.");
        }

        // 4. Verificar senha
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            // Registrar tentativa falha
            await RegisterLoginAttempt(user.Id, email, ip, userAgent, false, true, false, false);

            if (signInResult.IsLockedOut)
                throw new UnauthorizedAccessException("Conta bloqueada por muitas tentativas falhas");
            
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");
        }

        // 5. Verificar se 2FA está habilitado
        if (user.TwoFactorEnabled)
        {
            // Gerar token temporário para 2FA
            var tempToken = _jwtService.GenerateTemporaryToken(user.Id, user.Email!);

            // Registrar login bem-sucedido (parcial - aguardando 2FA)
            await RegisterLoginAttempt(user.Id, email, ip, userAgent, true, false, false, false);

            return new AuthResult
            {
                RequiresTwoFactor = true,
                TemporaryToken = tempToken,
                UserId = user.Id
            };
        }

        // 6. Gerar tokens JWT
        return await GenerateTokensAndSessionAsync(user, ip, userAgent);
    }

    /// <summary>
    /// Verifica código 2FA e retorna tokens definitivos
    /// </summary>
    public async Task<AuthResult> VerifyTwoFactorAsync(Guid userId, string code, string tempToken, string ip, string userAgent)
    {
        // 1. Validar token temporário
        var userIdFromToken = _jwtService.GetUserIdFromToken(tempToken);
        if (userIdFromToken != userId)
            throw new UnauthorizedAccessException("Token inválido ou expirado");

        // 2. Encontrar usuário
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // 3. Verificar código 2FA
        if (string.IsNullOrEmpty(user.TwoFactorSecret))
            throw new UnauthorizedAccessException("2FA não configurado para este usuário");

        var isValid = _twoFactorService.VerifyTotp(user.TwoFactorSecret, code);
        if (!isValid)
        {
            // Registrar tentativa falha de 2FA
            await RegisterLoginAttempt(user.Id, user.Email!, ip, userAgent, false, false, true, false);
            throw new UnauthorizedAccessException("Código 2FA inválido");
        }

        // 4. Gerar tokens JWT
        return await GenerateTokensAndSessionAsync(user, ip, userAgent);
    }

    /// <summary>
    /// Renova tokens usando refresh token
    /// </summary>
    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, string ip, string userAgent)
    {
        // 1. Validar refresh token (buscar sessão ativa)
        var session = _masterDbContext.UserSessions
            .FirstOrDefault(s => s.RefreshToken == refreshToken && s.IsActive && s.ExpiresAt > DateTime.UtcNow);

        if (session == null)
            throw new UnauthorizedAccessException("Refresh token inválido ou expirado");

        // 2. Encontrar usuário
        var user = await _userManager.FindByIdAsync(session.UserId.ToString())
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // 3. Verificar se usuário ainda está ativo
        if (!user.IsActive || await _userManager.IsLockedOutAsync(user))
        {
            session.IsActive = false;
            session.UpdatedAt = DateTime.UtcNow;
            await _masterDbContext.SaveChangesAsync();
            throw new UnauthorizedAccessException("Usuário inativo ou bloqueado");
        }

        // 4. Revogar sessão antiga
        session.IsActive = false;
        session.UpdatedAt = DateTime.UtcNow;

        // 5. Gerar novos tokens
        return await GenerateTokensAndSessionAsync(user, ip, userAgent);
    }

    /// <summary>
    /// Realiza logout e revoga todos os tokens
    /// </summary>
    public async Task LogoutAsync(Guid userId, string? refreshToken = null)
    {
        // 1. Se refreshToken específico, revogar apenas ele
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var session = _masterDbContext.UserSessions
                .FirstOrDefault(s => s.RefreshToken == refreshToken && s.UserId == userId);
            
            if (session != null)
            {
                session.IsActive = false;
                session.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // 2. Revogar todas as sessões do usuário
            var sessions = _masterDbContext.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToList();

            foreach (var session in sessions)
            {
                session.IsActive = false;
                session.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _masterDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Habilita 2FA para o usuário
    /// </summary>
    public async Task<TwoFactorSetupResult> EnableTwoFactorAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // Gerar secret TOTP
        var secret = _twoFactorService.GenerateTotpSecret();
        
        // Gerar códigos de recuperação
        var recoveryCodes = _twoFactorService.GenerateRecoveryCodes(10);
        
        // Gerar URI para QR Code
        var provisioningUri = _twoFactorService.GetTotpProvisioningUri(secret, user.Email!, "ERP SaaS");

        return new TwoFactorSetupResult
        {
            Secret = secret,
            RecoveryCodes = recoveryCodes,
            ProvisioningUri = provisioningUri
        };
    }

    /// <summary>
    /// Confirma e ativa 2FA para o usuário
    /// </summary>
    public async Task<bool> ConfirmTwoFactorAsync(Guid userId, string code, string secret)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // Validar código
        var isValid = _twoFactorService.VerifyTotp(secret, code);
        if (!isValid)
            return false;

        // Salvar secret e ativar 2FA
        user.TwoFactorSecret = secret;
        user.TwoFactorEnabled = true;
        user.RecoveryCodes = _twoFactorService.GenerateRecoveryCodes(10).ToList();
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        return true;
    }

    /// <summary>
    /// Desabilita 2FA para o usuário
    /// </summary>
    public async Task DisableTwoFactorAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        user.RecoveryCodes = new List<string>();
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
    }

    #region Private Methods

    /// <summary>
    /// Gera tokens JWT e cria sessão para o usuário
    /// </summary>
    private async Task<AuthResult> GenerateTokensAndSessionAsync(User user, string ip, string userAgent)
    {
        // 1. Obter roles e permissões
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);

        // 2. Gerar tokens JWT
        var accessToken = _jwtService.GenerateAccessToken(user, roles.ToList(), permissions, user.TenantId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 3. Criar sessão
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SessionId = Guid.NewGuid().ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ClientIp = ip,
            UserAgent = userAgent,
            TenantId = user.TenantId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            IsActive = true
        };
        _masterDbContext.UserSessions.Add(session);

        // 4. Registrar login bem-sucedido
        await RegisterLoginAttempt(user.Id, user.Email!, ip, userAgent, true, false, false, false);

        await _masterDbContext.SaveChangesAsync();

        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            UserId = user.Id,
            Email = user.Email!,
            TenantId = user.TenantId,
            Roles = roles.ToList(),
            Permissions = permissions,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    /// <summary>
    /// Obtém permissões do usuário baseado em suas roles
    /// </summary>
    private async Task<List<string>> GetUserPermissionsAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            var role = await _masterDbContext.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role != null)
            {
                permissions.AddRange(role.RolePermissions
                    .Select(rp => rp.Permission.Code)
                    .ToList());
            }
        }

        return permissions.Distinct().ToList();
    }

    /// <summary>
    /// Registra tentativa de login no banco
    /// </summary>
    private async Task RegisterLoginAttempt(Guid? userId, string email, string ip, string userAgent, 
        bool isSuccess, bool wrongPassword, bool twoFactorFailed, bool accountLocked)
    {
        var loginAttempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email,
            Username = email,
            ClientIp = ip,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            WrongPassword = wrongPassword,
            TwoFactorFailed = twoFactorFailed,
            AccountLocked = accountLocked,
            AttemptTime = DateTime.UtcNow
        };

        _masterDbContext.LoginAttempts.Add(loginAttempt);
        await _masterDbContext.SaveChangesAsync();
    }

    #endregion
}

/// <summary>
/// Resultado da autenticação
/// </summary>
public class AuthResult
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid? TenantId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public bool RequiresTwoFactor { get; set; }
    public string? TemporaryToken { get; set; }
}

/// <summary>
/// Resultado da configuração do 2FA
/// </summary>
public class TwoFactorSetupResult
{
    public string Secret { get; set; } = string.Empty;
    public string[] RecoveryCodes { get; set; } = Array.Empty<string>();
    public string ProvisioningUri { get; set; } = string.Empty;
}
