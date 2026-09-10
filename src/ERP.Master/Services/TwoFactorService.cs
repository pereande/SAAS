using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.Master.Models;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OtpNet;

namespace ERP.Master.Services;

/// <summary>
/// Serviço para autenticação de dois fatores (2FA) usando TOTP
/// </summary>
public class TwoFactorService
{
    private readonly TwoFactorSettings _settings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(
        IOptions<TwoFactorSettings> settings,
        UserManager<User> userManager,
        ILogger<TwoFactorService> logger)
    {
        _settings = settings.Value;
        _userManager = userManager;
        _logger = logger;
    }

    public (string Secret, string QrCodeUri) GenerateSecret(User user)
    {
        var secret = GenerateRandomSecret();
        var issuer = _settings.Issuer;
        var userEmail = user.Email ?? "user@erpsaas.com";

        var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(userEmail)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&algorithm=SHA1&digits=6&period=30";

        return (secret, qrCodeUri);
    }

    private string GenerateRandomSecret()
    {
        var key = new byte[20];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Base32Encoding.ToString(key);
    }

    public bool ValidateCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        try
        {
            var totp = new Totp(
                ByteArrayFromString(secret),
                step: _settings.CodeExpirySeconds,
                totpSize: 6,
                mode: OtpHashMode.Sha1);

            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating 2FA code");
            return false;
        }
    }

    public async Task<bool> ValidateUserCodeAsync(User user, string code)
    {
        if (user.TwoFactorSecret == null)
        {
            _logger.LogWarning("User {UserId} does not have 2FA secret", user.Id);
            return false;
        }

        return ValidateCode(user.TwoFactorSecret, code);
    }

    public List<string> GenerateRecoveryCodes(int count = 10, int length = 8)
    {
        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            codes.Add(GenerateRandomCode(length));
        }
        return codes;
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var code = new StringBuilder(length);

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        for (int i = 0; i < length; i++)
        {
            code.Append(chars[bytes[i] % chars.Length]);
        }

        return code.ToString();
    }

    public async Task<bool> ValidateRecoveryCodeAsync(User user, string code)
    {
        if (user.TwoFactorRecoveryCodes == null)
        {
            return false;
        }

        var codes = user.TwoFactorRecoveryCodes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var normalizedCode = code.Trim().ToUpper();

        for (int i = 0; i < codes.Length; i++)
        {
            if (codes[i].Trim().ToUpper() == normalizedCode)
            {
                codes[i] = string.Empty;
                user.TwoFactorRecoveryCodes = string.Join(",", codes);
                await _userManager.UpdateAsync(user);
                return true;
            }
        }

        return false;
    }

    private static byte[] ByteArrayFromString(string secret)
    {
        return Base32Encoding.ToBytes(secret);
    }
}
