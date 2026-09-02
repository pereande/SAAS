namespace ERP.Shared.Settings;

/// <summary>
/// Configurações do JWT
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Chave secreta para assinatura do token
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Emissor do token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiência do token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração do token (em minutos)
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// Tempo de expiração do refresh token (em dias)
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 30;

    /// <summary>
    /// Permitir refresh token
    /// </summary>
    public bool AllowRefreshToken { get; set; } = true;

    /// <summary>
    /// Validar emissor
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Validar audiência
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Validar tempo de vida
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Validar assinatura
    /// </summary>
    public bool ValidateSignature { get; set; } = true;
}

/// <summary>
/// Configurações do 2FA
/// </summary>
public class TwoFactorSettings
{
    /// <summary>
    /// Nome do emissor (para TOTP)
    /// </summary>
    public string Issuer { get; set; } = "ERP SaaS";

    /// <summary>
    /// Tempo de validade do código (em segundos)
    /// </summary>
    public int CodeExpirySeconds { get; set; } = 30;

    /// <summary>
    /// Número de códigos de recuperação
    /// </summary>
    public int RecoveryCodesCount { get; set; } = 10;

    /// <summary>
    /// Tamanho do código de recuperação
    /// </summary>
    public int RecoveryCodeLength { get; set; } = 8;

    /// <summary>
    /// Permitir 2FA
    /// </summary>
    public bool EnableTwoFactor { get; set; } = true;

    /// <summary>
    /// Obrigar 2FA para admins
    /// </summary>
    public bool RequireTwoFactorForAdmins { get; set; } = true;
}

/// <summary>
/// Configurações de segurança
/// </summary>
public class SecuritySettings
{
    /// <summary>
    /// Tempo de bloqueio após tentativas falhas (em minutos)
    /// </summary>
    public int LockoutMinutes { get; set; } = 30;

    /// <summary>
    /// Número máximo de tentativas falhas
    /// </summary>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    /// Permitir bloqueio de conta
    /// </summary>
    public bool AllowAccountLockout { get; set; } = true;

    /// <summary>
    /// Requerir senha forte
    /// </summary>
    public bool RequireStrongPassword { get; set; } = true;

    /// <summary>
    /// Tamanho mínimo da senha
    /// </summary>
    public int MinimumPasswordLength { get; set; } = 8;

    /// <summary>
    /// Requerir letra maiúscula
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Requerir letra minúscula
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Requerir número
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Requerir caractere especial
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    /// Requerir caracteres únicos
    /// </summary>
    public int RequiredUniqueChars { get; set; } = 1;
}

/// <summary>
/// Configurações de sessão
/// </summary>
public class SessionSettings
{
    /// <summary>
    /// Tempo de expiração da sessão (em minutos)
    /// </summary>
    public int SessionExpiryMinutes { get; set; } = 120;

    /// <summary>
    /// Permitir sessão persistente
    /// </summary>
    public bool AllowPersistentSession { get; set; } = true;

    /// <summary>
    /// Tempo de expiração da sessão persistente (em dias)
    /// </summary>
    public int PersistentSessionExpiryDays { get; set; } = 30;

    /// <summary>
    /// Limite de sessões por usuário
    /// </summary>
    public int MaxSessionsPerUser { get; set; } = 5;
}

/// <summary>
/// Configurações de rate limiting
/// </summary>
public class RateLimitSettings
{
    /// <summary>
    /// Habilitar rate limiting
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Limite de requisições por minuto
    /// </summary>
    public int RequestsPerMinute { get; set; } = 100;

    /// <summary>
    /// Limite de requisições por hora
    /// </summary>
    public int RequestsPerHour { get; set; } = 1000;

    /// <summary>
    /// Limite de requisições por dia
    /// </summary>
    public int RequestsPerDay { get; set; } = 10000;

    /// <summary>
    /// Tempo de bloqueio (em minutos)
    /// </summary>
    public int BlockDurationMinutes { get; set; } = 15;
}

/// <summary>
/// Configurações de CORS
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// Origens permitidas
    /// </summary>
    public List<string> AllowedOrigins { get; set; } = new List<string>();

    /// <summary>
    /// Métodos permitidos
    /// </summary>
    public List<string> AllowedMethods { get; set; } = new List<string> { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

    /// <summary>
    /// Headers permitidos
    /// </summary>
    public List<string> AllowedHeaders { get; set; } = new List<string> { "*" };

    /// <summary>
    /// Permitir credenciais
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Tempo de cache pré-flight (em segundos)
    /// </summary>
    public int PreflightCacheDurationSeconds { get; set; } = 1800;
}
