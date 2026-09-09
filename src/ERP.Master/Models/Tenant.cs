using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa um tenant (empresa contratante) da plataforma
/// Cada tenant possui seu próprio banco de dados (ERP_EMPRESA_XXX)
/// </summary>
[Table("tenants")]
public class Tenant : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome da empresa
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da empresa
    /// </summary>
    [Required]
    [Column("cnpj")]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// E-mail de contato
    /// </summary>
    [Required]
    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone de contato
    /// </summary>
    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Status do tenant
    /// </summary>
    [Required]
    [Column("status")]
    public TenantStatus Status { get; set; } = TenantStatus.Trial;

    /// <summary>
    /// Nome do banco de dados do tenant
    /// </summary>
    [Required]
    [Column("db_name")]
    [StringLength(100)]
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// Connection string do banco do tenant
    /// </summary>
    [Required]
    [Column("conn_string")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    [Column("max_users")]
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento (bytes)
    /// </summary>
    [Column("max_storage")]
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Armazenamento atual (bytes)
    /// </summary>
    [Required]
    [Column("current_storage")]
    public long CurrentStorage { get; set; } = 0;

    /// <summary>
    /// Data de fim do trial
    /// </summary>
    [Column("trial_end")]
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// ID da assinatura
    /// </summary>
    [Column("subscription_id")]
    public Guid? SubscriptionId { get; set; }

    /// <summary>
    /// Assinatura do tenant
    /// </summary>
    [ForeignKey("SubscriptionId")]
    public virtual Subscription? Subscription { get; set; }

    /// <summary>
    /// ID do plano contratado
    /// </summary>
    [Column("plan_id")]
    public Guid? PlanId { get; set; }

    /// <summary>
    /// Módulos habilitados para o tenant
    /// </summary>
    [Required]
    [Column("modules")]
    public List<string> EnabledModules { get; set; } = new List<string>();

    /// <summary>
    /// Configurações adicionais do tenant
    /// </summary>
    [Required]
    [Column("settings")]
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

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
    /// Usuários do tenant
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

/// <summary>
/// Status possíveis de um tenant
/// </summary>
public enum TenantStatus
{
    /// <summary>
    /// Em período de trial
    /// </summary>
    Trial = 0,

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
