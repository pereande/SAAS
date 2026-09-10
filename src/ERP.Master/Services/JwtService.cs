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

namespace ERP.Master.Services;

/// <summary>
/// Serviço para manipulação de tokens JWT
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<JwtService> _logger;
    private readonly IRepository<UserToken, Guid> _userTokenRepository;

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

        if (tenantId.HasValue)
        {
            claims.Add(new Claim("tenant_id", tenantId.Value.ToString()));
        }

        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

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

    public async Task<string> GenerateRefreshTokenAsync(
        User user,
        string? clientIp = null,
        string? userAgent = null)
    {
        var token = GenerateRandomToken();

        var refreshToken = new UserToken
        {
            UserId = user.Id,
            Type = TokenType.RefreshToken,
            TokenHash = HashToken(token),
            TokenOriginal = null,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false,
            CreatedIp = clientIp,
            UserAgent = userAgent
        };

        await _userTokenRepository.AddAsync(refreshToken);

        _logger.LogInformation("Refresh token generated for user {UserId}", user.Id);

        return token;
    }

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

    public async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                         principal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedException(ErrorMessages.InvalidToken);
        }

        return userId;
    }

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

    public async Task<IList<string>> GetRolesFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    private static string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

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
    Task<string> GenerateTokenAsync(
        User user,
        Guid? tenantId = null,
        IList<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null);

    Task<string> GenerateRefreshTokenAsync(
        User user,
        string? clientIp = null,
        string? userAgent = null);

    Task<ClaimsPrincipal> ValidateTokenAsync(string token);

    Task<bool> ValidateRefreshTokenAsync(string token, User user);

    Task<bool> RevokeRefreshTokenAsync(string token, User user, string? reason = null);

    Task<int> RevokeAllRefreshTokensAsync(User user, string? reason = null);

    Task<Guid> GetUserIdFromTokenAsync(string token);

    Task<Guid?> GetTenantIdFromTokenAsync(string token);

    Task<IList<string>> GetRolesFromTokenAsync(string token);
}
