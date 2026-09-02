using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa um plano de assinatura do ERP SaaS
/// </summary>
[Table("plans")]
public class Plan : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome do plano
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código do plano (ex: "basic", "pro", "enterprise")
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do plano
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Preço mensal (em centavos)
    /// </summary>
    [Required]
    [Column("monthly_price")]
    public decimal MonthlyPrice { get; set; }

    /// <summary>
    /// Preço anual (em centavos)
    /// </summary>
    [Column("yearly_price")]
    public decimal? YearlyPrice { get; set; }

    /// <summary>
    /// Período de trial em dias
    /// </summary>
    [Column("trial_days")]
    public int? TrialDays { get; set; } = 30;

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
    /// Limite máximo de armazenamento (bytes)
    /// </summary>
    [Column("max_storage")]
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Módulos incluídos
    /// </summary>
    [Required]
    [Column("modules")]
    public List<string> IncludedModules { get; set; } = new List<string>();

    /// <summary>
    /// Recursos incluídos
    /// </summary>
    [Column("features")]
    public List<string> IncludedFeatures { get; set; } = new List<string>();

    /// <summary>
    /// Plano ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Plano padrão
    /// </summary>
    [Required]
    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Ordem de exibição
    /// </summary>
    [Column("display_order")]
    public int DisplayOrder { get; set; } = 0;

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
    /// Assinaturas com este plano
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

/// <summary>
/// Módulos disponíveis no ERP
/// </summary>
public static class ERPModules
{
    /// <summary>
    /// Módulo de Vendas
    /// </summary>
    public const string Sales = "sales";

    /// <summary>
    /// Módulo de Compras
    /// </summary>
    public const string Purchases = "purchases";

    /// <summary>
    /// Módulo de Estoque
    /// </summary>
    public const string Inventory = "inventory";

    /// <summary>
    /// Módulo Fiscal
    /// </summary>
    public const string Fiscal = "fiscal";

    /// <summary>
    /// Módulo Financeiro
    /// </summary>
    public const string Financial = "financial";

    /// <summary>
    /// Módulo de CRM
    /// </summary>
    public const string CRM = "crm";

    /// <summary>
    /// Módulo de RH
    /// </summary>
    public const string HR = "hr";

    /// <summary>
    /// Módulo de Relatórios
    /// </summary>
    public const string Reports = "reports";

    /// <summary>
    /// Módulo de Dashboard
    /// </summary>
    public const string Dashboard = "dashboard";

    /// <summary>
    /// Módulo de API
    /// </summary>
    public const string API = "api";

    /// <summary>
    /// Todos os módulos
    /// </summary>
    public static readonly List<string> All = new List<string>
    {
        Sales, Purchases, Inventory, Fiscal, Financial, CRM, HR, Reports, Dashboard, API
    };
}
