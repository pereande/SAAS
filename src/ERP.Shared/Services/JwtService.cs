using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.Master.Models;
using ERP.Shared.Exceptions;
using ERP.Shared.Interfaces;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ERP.Shared.Services;

/// <summary>
/// Serviço para manipulação de tokens JWT
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<JwtService> _logger;
    private readonly IRepository<UserToken, Guid> _userTokenRepository;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="jwtSettings">Configurações do JWT</param>
    /// <param name="userManager">UserManager do Identity</param>
    /// <param name="logger">Logger</param>
    /// <param name="userTokenRepository">Repositório de tokens do usuário</param>
    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        UserManager<User> userManager,
        ILogger<JwtService> logger,
        IRepository<UserToken, Guid> userTokenRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _logger = logger;
        _userTokenRepository = userTokenRepository;
    }

    /// <summary>
    /// Gera um token JWT
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="tenantId">ID do tenant (opcional)</param>
    /// <param name="roles">Perfis do usuário</param>
    /// <param name="additionalClaims">Claims adicionais</param>
    /// <returns>Token JWT</returns>
    public async Task<string> GenerateTokenAsync(
        User user,
        Guid? tenantId = null,
        IList<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("user_id", user.Id.ToString()),
            new Claim("email", user.Email ?? string.Empty),
            new Claim("username", user.UserName ?? string.Empty),
            new Claim("first_name", user.FirstName),
            new Claim("last_name", user.LastName),
            new Claim("full_name", user.FullName),
            new Claim("locale", user.Locale),
            new Claim("timezone", user.Timezone)
        };

        // Adicionar tenant
        if (tenantId.HasValue)
        {
            claims.Add(new Claim("tenant_id", tenantId.Value.ToString()));
        }

        // Adicionar roles
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        // Adicionar claims adicionais
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            NotBefore = DateTime.UtcNow
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="clientIp">IP do cliente</param>
    /// <param name="userAgent">User Agent</param>
    /// <returns>Refresh token</returns>
    public async Task<string> GenerateRefreshTokenAsync(
        User user,
        string? clientIp = null,
        string? userAgent = null)
    {
        // Gerar token aleatório
        var token = GenerateRandomToken();

        // Salvar refresh token no banco
        var refreshToken = new UserToken
        {
            UserId = user.Id,
            Type = TokenType.RefreshToken,
            TokenHash = HashToken(token),
            TokenOriginal = null, // Não salvar token original
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false,
            CreatedIp = clientIp,
            UserAgent = userAgent
        };

        await _userTokenRepository.AddAsync(refreshToken);

        _logger.LogInformation("Refresh token generated for user {UserId}", user.Id);

        return token;
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ClaimsPrincipal</returns>
    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidateLifetime = _jwtSettings.ValidateLifetime,
            ValidateIssuerSigningKey = _jwtSettings.ValidateSignature,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning(ex, "Token expired");
            throw new UnauthorizedException(ErrorMessages.ExpiredToken, ex);
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning(ex, "Invalid token signature");
            throw new UnauthorizedException(ErrorMessages.InvalidToken, ex);
        }
        catch (SecurityTokenInvalidAudienceException ex)
        {
            _logger.LogWarning(ex, "Invalid token audience");
            throw new UnauthorizedException(ErrorMessages.InvalidToken, ex);
        }
        catch (SecurityTokenInvalidIssuerException ex)
        {
            _logger.LogWarning(ex, "Invalid token issuer");
            throw new UnauthorizedException(ErrorMessages.InvalidToken, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            throw new UnauthorizedException(ErrorMessages.InvalidToken, ex);
        }
    }

    /// <summary>
    /// Valida um refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="user">Usuário</param>
    /// <returns>True se válido</returns>
    public async Task<bool> ValidateRefreshTokenAsync(string token, User user)
    {
        var tokenHash = HashToken(token);

        var userToken = await _userTokenRepository.GetFirstOrDefaultAsync(
            t => t.UserId == user.Id && 
                 t.TokenHash == tokenHash && 
                 t.Type == TokenType.RefreshToken &&
                 !t.IsRevoked);

        if (userToken == null)
        {
            _logger.LogWarning("Refresh token not found for user {UserId}", user.Id);
            return false;
        }

        if (userToken.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired for user {UserId}", user.Id);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Revoga um refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="user">Usuário</param>
    /// <param name="reason">Motivo da revogação</param>
    /// <returns>True se revogado</returns>
    public async Task<bool> RevokeRefreshTokenAsync(string token, User user, string? reason = null)
    {
        var tokenHash = HashToken(token);

        var userToken = await _userTokenRepository.GetFirstOrDefaultAsync(
            t => t.UserId == user.Id && 
                 t.TokenHash == tokenHash && 
                 t.Type == TokenType.RefreshToken);

        if (userToken == null)
        {
            _logger.LogWarning("Refresh token not found for revocation for user {UserId}", user.Id);
            return false;
        }

        userToken.IsRevoked = true;
        userToken.RevokedAt = DateTime.UtcNow;
        userToken.RevokedReason = reason ?? "Revoked by user request";

        await _userTokenRepository.UpdateAsync(userToken);

        _logger.LogInformation("Refresh token revoked for user {UserId}. Reason: {Reason}", user.Id, reason);

        return true;
    }

    /// <summary>
    /// Revoga todos os refresh tokens do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="reason">Motivo da revogação</param>
    /// <returns>Número de tokens revogados</returns>
    public async Task<int> RevokeAllRefreshTokensAsync(User user, string? reason = null)
    {
        var tokens = await _userTokenRepository.GetWhereAsync(
            t => t.UserId == user.Id && 
                 t.Type == TokenType.RefreshToken &&
                 !t.IsRevoked);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = reason ?? "All tokens revoked";
        }

        if (tokens.Count > 0)
        {
            await _userTokenRepository.UpdateRangeAsync(tokens);
            _logger.LogInformation("All refresh tokens revoked for user {UserId}. Count: {Count}. Reason: {Reason}", 
                user.Id, tokens.Count, reason);
        }

        return tokens.Count;
    }

    /// <summary>
    /// Obtém o ID do usuário do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID do usuário</returns>
    public async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?:
                         principal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedException(ErrorMessages.InvalidToken);
        }

        return userId;
    }

    /// <summary>
    /// Obtém o ID do tenant do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID do tenant (null se não houver)</returns>
    public async Task<Guid?> GetTenantIdFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        var tenantIdClaim = principal.FindFirst("tenant_id");

        if (tenantIdClaim == null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return null;
        }

        return tenantId;
    }

    /// <summary>
    /// Obtém os roles do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Lista de roles</returns>
    public async Task<IList<string>> GetRolesFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    /// <summary>
    /// Gera um token aleatório
    /// </summary>
    /// <returns>Token aleatório</returns>
    private static string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Faz hash do token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Hash do token</returns>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}

/// <summary>
/// Interface do serviço de JWT
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Gera um token JWT
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="tenantId">ID do tenant (opcional)</param>
    /// <param name="roles">Perfis do usuário</param>
    /// <param name="additionalClaims">Claims adicionais</param>
    /// <returns>Token JWT</returns>
    Task<string> GenerateTokenAsync(
        User user,
        Guid? tenantId = null,
        IList<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="clientIp">IP do cliente</param>
    /// <param name="userAgent">User Agent</param>
    /// <returns>Refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(
        User user,
        string? clientIp = null,
        string? userAgent = null);

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ClaimsPrincipal</returns>
    Task<ClaimsPrincipal> ValidateTokenAsync(string token);

    /// <summary>
    /// Valida um refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="user">Usuário</param>
    /// <returns>True se válido</returns>
    Task<bool> ValidateRefreshTokenAsync(string token, User user);

    /// <summary>
    /// Revoga um refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="user">Usuário</param>
    /// <param name="reason">Motivo da revogação</param>
    /// <returns>True se revogado</returns>
    Task<bool> RevokeRefreshTokenAsync(string token, User user, string? reason = null);

    /// <summary>
    /// Revoga todos os refresh tokens do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="reason">Motivo da revogação</param>
    /// <returns>Número de tokens revogados</returns>
    Task<int> RevokeAllRefreshTokensAsync(User user, string? reason = null);

    /// <summary>
    /// Obtém o ID do usuário do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID do usuário</returns>
    Task<Guid> GetUserIdFromTokenAsync(string token);

    /// <summary>
    /// Obtém o ID do tenant do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID do tenant (null se não houver)</returns>
    Task<Guid?> GetTenantIdFromTokenAsync(string token);

    /// <summary>
    /// Obtém os roles do token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Lista de roles</returns>
    Task<IList<string>> GetRolesFromTokenAsync(string token);
}
