using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Assinatura de um tenant
/// </summary>
[Table("subscriptions")]
public class Subscription : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Tenant
    /// </summary>
    [ForeignKey("TenantId")]
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// ID do plano
    /// </summary>
    [Required]
    [Column("plan_id")]
    public Guid PlanId { get; set; }

    /// <summary>
    /// Plano
    /// </summary>
    [ForeignKey("PlanId")]
    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// Status
    /// </summary>
    [Required]
    [Column("status")]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    /// <summary>
    /// Data de início
    /// </summary>
    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de fim
    /// </summary>
    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Início do trial
    /// </summary>
    [Column("trial_start")]
    public DateTime? TrialStart { get; set; }

    /// <summary>
    /// Fim do trial
    /// </summary>
    [Column("trial_end")]
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Método de pagamento
    /// </summary>
    [Column("payment_method")]
    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    [Column("payment_status")]
    [StringLength(20)]
    public string? PaymentStatus { get; set; }

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    [Column("max_users")]
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de filiais
    /// </summary>
    [Column("max_filials")]
    public int? MaxFilials { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento
    /// </summary>
    [Column("max_storage")]
    public long? MaxStorage { get; set; }

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
}

/// <summary>
/// Status da assinatura
/// </summary>
public enum SubscriptionStatus
{
    Active = 0,
    Canceled = 1,
    Expired = 2,
    PastDue = 3,
    Trialing = 4,
    Suspended = 5
}
