using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa o estoque de um produto em uma filial
/// </summary>
[Table("inventories")]
public class Inventory : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da filial
    /// </summary>
    [Required]
    [Column("branch_id")]
    public Guid BranchId { get; set; }

    /// <summary>
    /// Filial
    /// </summary>
    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// ID do produto
    /// </summary>
    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// Produto
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Quantidade atual
    /// </summary>
    [Required]
    [Column("quantity")]
    public decimal Quantity { get; set; } = 0;

    /// <summary>
    /// Quantidade reservada
    /// </summary>
    [Required]
    [Column("reserved_quantity")]
    public decimal ReservedQuantity { get; set; } = 0;

    /// <summary>
    /// Quantidade disponível
    /// </summary>
    [NotMapped]
    public decimal AvailableQuantity => Quantity - ReservedQuantity;

    /// <summary>
    /// Custo médio
    /// </summary>
    [Column("average_cost")]
    public decimal? AverageCost { get; set; }

    /// <summary>
    /// Localização no estoque
    /// </summary>
    [Column("location")]
    [StringLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// Data da última entrada
    /// </summary>
    [Column("last_entry_date")]
    public DateTime? LastEntryDate { get; set; }

    /// <summary>
    /// Data da última saída
    /// </summary>
    [Column("last_exit_date")]
    public DateTime? LastExitDate { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Representa uma movimentação de estoque
/// </summary>
[Table("inventory_movements")]
public class InventoryMovement : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID do estoque
    /// </summary>
    [Column("inventory_id")]
    public Guid? InventoryId { get; set; }

    /// <summary>
    /// Estoque
    /// </summary>
    [ForeignKey("InventoryId")]
    public virtual Inventory? Inventory { get; set; }

    /// <summary>
    /// ID da filial
    /// </summary>
    [Required]
    [Column("branch_id")]
    public Guid BranchId { get; set; }

    /// <summary>
    /// Filial
    /// </summary>
    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// ID do produto
    /// </summary>
    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// Produto
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Tipo de movimento
    /// </summary>
    [Required]
    [Column("movement_type")]
    [StringLength(20)]
    public InventoryMovementType MovementType { get; set; } = InventoryMovementType.Entry;

    /// <summary>
    /// Quantidade
    /// </summary>
    [Required]
    [Column("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Quantidade anterior
    /// </summary>
    [Column("previous_quantity")]
    public decimal? PreviousQuantity { get; set; }

    /// <summary>
    /// Quantidade nova
    /// </summary>
    [Column("new_quantity")]
    public decimal? NewQuantity { get; set; }

    /// <summary>
    /// Custo unitário
    /// </summary>
    [Column("unit_cost")]
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Custo total
    /// </summary>
    [Column("total_cost")]
    public decimal? TotalCost { get; set; }

    /// <summary>
    /// ID da venda (se aplicável)
    /// </summary>
    [Column("sale_id")]
    public Guid? SaleId { get; set; }

    /// <summary>
    /// Venda
    /// </summary>
    [ForeignKey("SaleId")]
    public virtual Sale? Sale { get; set; }

    /// <summary>
    /// ID da compra (se aplicável)
    /// </summary>
    [Column("purchase_id")]
    public Guid? PurchaseId { get; set; }

    /// <summary>
    /// Compra
    /// </summary>
    [ForeignKey("PurchaseId")]
    public virtual Purchase? Purchase { get; set; }

    /// <summary>
    /// ID do item de venda (se aplicável)
    /// </summary>
    [Column("sale_item_id")]
    public Guid? SaleItemId { get; set; }

    /// <summary>
    /// ID do item de compra (se aplicável)
    /// </summary>
    [Column("purchase_item_id")]
    public Guid? PurchaseItemId { get; set; }

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// ID do usuário que executou a ação
    /// </summary>
    [Column("user_id")]
    [StringLength(450)]
    public string? UserId { get; set; }

    /// <summary>
    /// IP do usuário
    /// </summary>
    [Column("user_ip")]
    [StringLength(45)]
    public string? UserIp { get; set; }
}

/// <summary>
/// Tipo de movimento de estoque
/// </summary>
public enum InventoryMovementType
{
    /// <summary>
    /// Entrada
    /// </summary>
    Entry = 0,

    /// <summary>
    /// Saída
    /// </summary>
    Exit = 1,

    /// <summary>
    /// Ajuste positivo
    /// </summary>
    AdjustmentIn = 2,

    /// <summary>
    /// Ajuste negativo
    /// </summary>
    AdjustmentOut = 3,

    /// <summary>
    /// Transferência entre filiais (saída)
    /// </summary>
    TransferOut = 4,

    /// <summary>
    /// Transferência entre filiais (entrada)
    /// </summary>
    TransferIn = 5,

    /// <summary>
    /// Devolução de venda
    /// </summary>
    SalesReturn = 6,

    /// <summary>
    /// Devolução de compra
    /// </summary>
    PurchaseReturn = 7
}
