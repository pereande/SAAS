using Microsoft.AspNetCore.Mvc;
using ERP.Shared.Services;
using System.Security.Claims;

namespace ERP.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login com email e senha
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var ip = GetClientIp();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.LoginAsync(dto.Email, dto.Password, ip, userAgent);

            if (result.RequiresTwoFactor)
            {
                return Ok(new
                {
                    requiresTwoFactor = true,
                    temporaryToken = result.TemporaryToken,
                    userId = result.UserId
                });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            return StatusCode(500, new { message = "Erro interno ao processar login" });
        }
    }

    /// <summary>
    /// Verifica código 2FA após login
    /// </summary>
    [HttpPost("verify-2fa")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorDto dto)
    {
        try
        {
            var ip = GetClientIp();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.VerifyTwoFactorAsync(
                dto.UserId, 
                dto.Code, 
                dto.TemporaryToken, 
                ip, 
                userAgent
            );

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na verificação 2FA");
            return StatusCode(500, new { message = "Erro interno ao verificar 2FA" });
        }
    }

    /// <summary>
    /// Renova tokens usando refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var ip = GetClientIp();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ip, userAgent);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(500, new { message = "Erro interno ao renovar token" });
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] LogoutDto? dto = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.LogoutAsync(userId, dto?.RefreshToken);
            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no logout");
            return StatusCode(500, new { message = "Erro interno ao realizar logout" });
        }
    }

    /// <summary>
    /// Habilita 2FA para o usuário atual
    /// </summary>
    [HttpPost("enable-2fa")]
    [Authorize]
    [ProducesResponseType(typeof(TwoFactorSetupResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableTwoFactor()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.EnableTwoFactorAsync(userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao habilitar 2FA");
            return StatusCode(500, new { message = "Erro interno ao habilitar 2FA" });
        }
    }

    /// <summary>
    /// Confirma e ativa 2FA
    /// </summary>
    [HttpPost("confirm-2fa")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmTwoFactor([FromBody] ConfirmTwoFactorDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _authService.ConfirmTwoFactorAsync(userId, dto.Code, dto.Secret);
            
            if (!success)
                return BadRequest(new { message = "Código 2FA inválido" });

            return Ok(new { message = "2FA ativado com sucesso" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar 2FA");
            return StatusCode(500, new { message = "Erro interno ao confirmar 2FA" });
        }
    }

    /// <summary>
    /// Desabilita 2FA para o usuário atual
    /// </summary>
    [HttpPost("disable-2fa")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DisableTwoFactor()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.DisableTwoFactorAsync(userId);
            return Ok(new { message = "2FA desativado com sucesso" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desabilitar 2FA");
            return StatusCode(500, new { message = "Erro interno ao desabilitar 2FA" });
        }
    }

    #region Helper Methods

    private string GetClientIp()
    {
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                 ?? Request.HttpContext.Connection.RemoteIpAddress?.ToString() 
                 ?? "unknown";
        return ip;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Usuário não autenticado");
        
        return userId;
    }

    #endregion
}

#region DTOs

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VerifyTwoFactorDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string TemporaryToken { get; set; } = string.Empty;
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LogoutDto
{
    public string? RefreshToken { get; set; }
}

public class ConfirmTwoFactorDto
{
    public string Code { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}

#endregion
