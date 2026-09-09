using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa a assinatura de um tenant
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
    /// ID do plano contratado
    /// </summary>
    [Required]
    [Column("plan_id")]
    public Guid PlanId { get; set; }

    /// <summary>
    /// Plano contratado
    /// </summary>
    [ForeignKey("PlanId")]
    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// Tenant da assinatura
    /// </summary>
    public virtual Tenant? Tenant { get; set; }

    /// <summary>
    /// Status da assinatura
    /// </summary>
    [Required]
    [Column("status")]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;

    /// <summary>
    /// Data de início da assinatura
    /// </summary>
    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de término da assinatura (null = vigente)
    /// </summary>
    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Data de início do trial
    /// </summary>
    [Column("trial_start")]
    public DateTime? TrialStart { get; set; }

    /// <summary>
    /// Data de fim do trial
    /// </summary>
    [Column("trial_end")]
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Método de pagamento
    /// </summary>
    [Column("payment_method")]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Status do pagamento
    /// </summary>
    [Column("payment_status")]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Limite máximo de usuários (sobrepõe o plano, se definido)
    /// </summary>
    [Column("max_users")]
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de filiais (sobrepõe o plano, se definido)
    /// </summary>
    [Column("max_filials")]
    public int? MaxFilials { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento (sobrepõe o plano, se definido)
    /// </summary>
    [Column("max_storage")]
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Assinatura ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

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
/// Status possíveis de uma assinatura
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Em período de trial
    /// </summary>
    Trial = 0,

    /// <summary>
    /// Ativa
    /// </summary>
    Active = 1,

    /// <summary>
    /// Pagamento pendente/atrasado
    /// </summary>
    PastDue = 2,

    /// <summary>
    /// Cancelada
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Expirada
    /// </summary>
    Expired = 4
}
