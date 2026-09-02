using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.Master.Models;
using ERP.Shared.Exceptions;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OtpNet;

namespace ERP.Shared.Services;

/// <summary>
/// Serviço para autenticação de dois fatores (2FA) usando TOTP
/// </summary>
public class TwoFactorService
{
    private readonly TwoFactorSettings _settings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TwoFactorService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="settings">Configurações de 2FA</param>
    /// <param name="userManager">UserManager do Identity</param>
    /// <param name="logger">Logger</param>
    public TwoFactorService(
        IOptions<TwoFactorSettings> settings,
        UserManager<User> userManager,
        ILogger<TwoFactorService> logger)
    {
        _settings = settings.Value;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Gera um segredo para 2FA
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <returns>Segredo e URI para configuração</returns>
    public (string Secret, string QrCodeUri) GenerateSecret(User user)
    {
        var secret = GenerateRandomSecret();
        var issuer = _settings.Issuer;
        var userEmail = user.Email ?? "user@erpsaas.com";

        // Gerar URI para o QR Code (otpauth://)
        var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(userEmail)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&algorithm=SHA1&digits=6&period=30";

        return (secret, qrCodeUri);
    }

    /// <summary>
    /// Gera um segredo aleatório
    /// </summary>
    /// <returns>Segredo base32</returns>
    private string GenerateRandomSecret()
    {
        var key = new byte[20]; // 160 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Base32Encoding.ToString(key);
    }

    /// <summary>
    /// Valida um código 2FA
    /// </summary>
    /// <param name="secret">Segredo</param>
    /// <param name="code">Código digitado pelo usuário</param>
    /// <returns>True se o código for válido</returns>
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

    /// <summary>
    /// Valida um código 2FA para um usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="code">Código digitado pelo usuário</param>
    /// <returns>True se o código for válido</returns>
    public async Task<bool> ValidateUserCodeAsync(User user, string code)
    {
        if (user.TwoFactorSecret == null)
        {
            _logger.LogWarning("User {UserId} does not have 2FA secret", user.Id);
            return false;
        }

        return ValidateCode(user.TwoFactorSecret, code);
    }

    /// <summary>
    /// Gera códigos de recuperação
    /// </summary>
    /// <param name="count">Número de códigos</param>
    /// <param name="length">Tamanho de cada código</param>
    /// <returns>Lista de códigos de recuperação</returns>
    public List<string> GenerateRecoveryCodes(int count = 10, int length = 8)
    {
        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            codes.Add(GenerateRandomCode(length));
        }
        return codes;
    }

    /// <summary>
    /// Gera um código aleatório
    /// </summary>
    /// <param name="length">Tamanho do código</param>
    /// <returns>Código aleatório</returns>
    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluindo caracteres ambíguos
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

    /// <summary>
    /// Valida um código de recuperação
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="code">Código de recuperação</param>
    /// <returns>True se o código for válido</returns>
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
                // Remover o código usado
                codes[i] = string.Empty;
                user.TwoFactorRecoveryCodes = string.Join(",", codes);
                await _userManager.UpdateAsync(user);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Converte string para byte array
    /// </summary>
    /// <param name="secret">Segredo base32</param>
    /// <returns>Byte array</returns>
    private static byte[] ByteArrayFromString(string secret)
    {
        return Base32Encoding.ToBytes(secret);
    }
}
