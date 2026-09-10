using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Usuário do sistema
/// </summary>
[Table("users")]
public class User : IdentityUser<Guid>
{
    /// <summary>
    /// Primeiro nome
    /// </summary>
    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    [Column("last_name")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// URL do avatar
    /// </summary>
    [Column("avatar_url")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Localidade (ex: pt-BR)
    /// </summary>
    [Column("locale")]
    [StringLength(10)]
    public string Locale { get; set; } = "pt-BR";

    /// <summary>
    /// Fuso horário
    /// </summary>
    [Column("timezone")]
    [StringLength(50)]
    public string Timezone { get; set; } = "America/Sao_Paulo";

    /// <summary>
    /// Status do usuário
    /// </summary>
    [Column("status")]
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// E-mail verificado
    /// </summary>
    [Column("email_verified")]
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// ID do tenant
    /// </summary>
    [Column("tenant_id")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Tenant
    /// </summary>
    [ForeignKey("TenantId")]
    public virtual Tenant? Tenant { get; set; }

    /// <summary>
    /// Segredo do 2FA
    /// </summary>
    [Column("two_factor_secret")]
    [StringLength(500)]
    public string? TwoFactorSecret { get; set; }

    /// <summary>
    /// Códigos de recuperação do 2FA
    /// </summary>
    [Column("two_factor_recovery_codes")]
    public string? TwoFactorRecoveryCodes { get; set; }

    /// <summary>
    /// Último login
    /// </summary>
    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// IP do último login
    /// </summary>
    [Column("last_login_ip")]
    [StringLength(45)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Tentativas de login falhas
    /// </summary>
    [Column("failed_login_attempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de atualização
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Roles do usuário
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

/// <summary>
/// Status do usuário
/// </summary>
public enum UserStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2,
    Pending = 3,
    Deleted = 4
}
