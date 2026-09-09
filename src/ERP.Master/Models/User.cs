using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Representa um usuário da plataforma (estende o IdentityUser)
/// </summary>
[Table("users")]
public class User : IdentityUser<Guid>, IEntity<Guid>
{
    /// <summary>
    /// Primeiro nome
    /// </summary>
    [Required]
    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    [Required]
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
    /// E-mail verificado
    /// </summary>
    [Required]
    [Column("email_verified")]
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// Localidade (ex: pt-BR)
    /// </summary>
    [Required]
    [Column("locale")]
    [StringLength(10)]
    public string Locale { get; set; } = "pt-BR";

    /// <summary>
    /// Fuso horário (ex: America/Sao_Paulo)
    /// </summary>
    [Required]
    [Column("timezone")]
    [StringLength(50)]
    public string Timezone { get; set; } = "America/Sao_Paulo";

    /// <summary>
    /// ID do tenant ao qual o usuário pertence (null para usuários da plataforma)
    /// </summary>
    [Column("tenant_id")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Tenant do usuário
    /// </summary>
    [ForeignKey("TenantId")]
    public virtual Tenant? Tenant { get; set; }

    /// <summary>
    /// Status do usuário
    /// </summary>
    [Required]
    [Column("status")]
    public UserStatus Status { get; set; } = UserStatus.Pending;

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
    /// Número de tentativas de login falhadas
    /// </summary>
    [Required]
    [Column("failed_login_attempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Segredo 2FA (TOTP, base32)
    /// </summary>
    [Column("two_factor_secret")]
    [StringLength(500)]
    public string? TwoFactorSecret { get; set; }

    /// <summary>
    /// Códigos de recuperação 2FA (separados por vírgula)
    /// </summary>
    [Column("two_factor_recovery_codes")]
    public string? TwoFactorRecoveryCodes { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de atualização
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Perfis do usuário
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

/// <summary>
/// Status possíveis de um usuário
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Aguardando ativação
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Ativo
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inativo
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Suspenso
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Excluído (soft delete)
    /// </summary>
    Deleted = 4
}
