using System;
using System.Collections.Generic;

namespace ERP.Web.DTOs.Auth;

/// <summary>
/// Request de login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Usuário ou e-mail
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Senha
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Request de login com 2FA
/// </summary>
public class LoginTwoFactorRequest
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Código 2FA
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request de refresh token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Token JWT
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request para habilitar 2FA
/// </summary>
public class EnableTwoFactorRequest
{
    /// <summary>
    /// Segredo
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Código
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request para desabilitar 2FA
/// </summary>
public class DisableTwoFactorRequest
{
    /// <summary>
    /// Senha
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response de login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Tempo de expiração (segundos)
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Requere 2FA
    /// </summary>
    public bool RequiresTwoFactor { get; set; }

    /// <summary>
    /// Token temporário para 2FA
    /// </summary>
    public string? TwoFactorToken { get; set; }

    /// <summary>
    /// ID do usuário
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário
    /// </summary>
    public UserDto? User { get; set; }
}

/// <summary>
/// Response de refresh token
/// </summary>
public class RefreshTokenResponse
{
    /// <summary>
    /// Token JWT
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração (segundos)
    /// </summary>
    public int ExpiresIn { get; set; }
}

/// <summary>
/// Response de 2FA
/// </summary>
public class TwoFactorResponse
{
    /// <summary>
    /// Habilitado
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Códigos de recuperação
    /// </summary>
    public List<string>? RecoveryCodes { get; set; }
}

/// <summary>
/// Response de segredo 2FA
/// </summary>
public class TwoFactorSecretResponse
{
    /// <summary>
    /// Segredo
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// URI do QR Code
    /// </summary>
    public string QrCodeUri { get; set; } = string.Empty;
}

/// <summary>
/// DTO do usuário
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Usuário
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail verificado
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// 2FA habilitado
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Telefone
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// URL do avatar
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Localidade
    /// </summary>
    public string Locale { get; set; } = "pt-BR";

    /// <summary>
    /// Fuso horário
    /// </summary>
    public string Timezone { get; set; } = "America/Sao_Paulo";

    /// <summary>
    /// Roles
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// Último login
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
