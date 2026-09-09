using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Representa um token de usuário (refresh token, reset password, etc.)
/// Estende o IdentityUserToken para compatibilidade com o IdentityDbContext
/// </summary>
[Table("user_tokens")]
public class UserToken : IdentityUserToken<Guid>, IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Usuário (UserId é herdado do IdentityUserToken)
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Tipo do token
    /// </summary>
    [Required]
    [Column("token_type")]
    [StringLength(50)]
    public TokenType Type { get; set; } = TokenType.RefreshToken;

    /// <summary>
    /// Token (hash)
    /// </summary>
    [Required]
    [Column("token_hash")]
    [StringLength(500)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Token original (para debug, opcional)
    /// </summary>
    [Column("token_original")]
    [StringLength(1000)]
    public string? TokenOriginal { get; set; }

    /// <summary>
    /// Data de expiração
    /// </summary>
    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token revogado
    /// </summary>
    [Required]
    [Column("is_revoked")]
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// Data de revogação
    /// </summary>
    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Motivo da revogação
    /// </summary>
    [Column("revoked_reason")]
    [StringLength(500)]
    public string? RevokedReason { get; set; }

    /// <summary>
    /// IP de criação
    /// </summary>
    [Column("created_ip")]
    [StringLength(45)]
    public string? CreatedIp { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    [Column("user_agent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Verifica se o token está expirado
    /// </summary>
    [NotMapped]
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    /// <summary>
    /// Verifica se o token é válido
    /// </summary>
    [NotMapped]
    public bool IsValid => !IsRevoked && !IsExpired;
}

/// <summary>
/// Tipos de token
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Refresh Token
    /// </summary>
    RefreshToken = 0,

    /// <summary>
    /// Token de reset de senha
    /// </summary>
    ResetPassword = 1,

    /// <summary>
    /// Token de confirmação de email
    /// </summary>
    ConfirmEmail = 2,

    /// <summary>
    /// Token de 2FA
    /// </summary>
    TwoFactor = 3
}
