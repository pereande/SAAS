using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma compra
/// </summary>
[Table("purchases")]
public class Purchase : BaseEntity<Guid>, ITenantEntity
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
    /// Número da compra
    /// </summary>
    [Required]
    [Column("number")]
    [StringLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// ID do fornecedor
    /// </summary>
    [Required]
    [Column("supplier_id")]
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Fornecedor
    /// </summary>
    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// Data da compra
    /// </summary>
    [Required]
    [Column("purchase_date")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de entrada (recebimento)
    /// </summary>
    [Column("entry_date")]
    public DateTime? EntryDate { get; set; }

    /// <summary>
    /// Status da compra
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;

    /// <summary>
    /// Tipo de compra
    /// </summary>
    [Required]
    [Column("purchase_type")]
    [StringLength(20)]
    public PurchaseType PurchaseType { get; set; } = PurchaseType.Purchase;

    /// <summary>
    /// ID da condição de pagamento
    /// </summary>
    [Column("payment_term_id")]
    public Guid? PaymentTermId { get; set; }

    /// <summary>
    /// Condição de pagamento
    /// </summary>
    [ForeignKey("PaymentTermId")]
    public virtual PaymentTerm? PaymentTerm { get; set; }

    /// <summary>
    /// ID do comprador
    /// </summary>
    [Column("buyer_id")]
    public Guid? BuyerId { get; set; }

    /// <summary>
    /// Comprador
    /// </summary>
    [ForeignKey("BuyerId")]
    public virtual Employee? Buyer { get; set; }

    /// <summary>
    /// Subtotal
    /// </summary>
    [Required]
    [Column("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    /// <summary>
    /// Desconto
    /// </summary>
    [Required]
    [Column("discount")]
    public decimal Discount { get; set; } = 0;

    /// <summary>
    /// Desconto (%)
    /// </summary>
    [Column("discount_percent")]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// Frete
    /// </summary>
    [Column("freight")]
    public decimal? Freight { get; set; }

    /// <summary>
    /// Outras despesas
    /// </summary>
    [Column("other_expenses")]
    public decimal? OtherExpenses { get; set; }

    /// <summary>
    /// Total de ICMS
    /// </summary>
    [Column("icms_total")]
    public decimal? IcmsTotal { get; set; }

    /// <summary>
    /// Total de IPI
    /// </summary>
    [Column("ipi_total")]
    public decimal? IpiTotal { get; set; }

    /// <summary>
    /// Total de PIS
    /// </summary>
    [Column("pis_total")]
    public decimal? PisTotal { get; set; }

    /// <summary>
    /// Total de COFINS
    /// </summary>
    [Column("cofins_total")]
    public decimal? CofinsTotal { get; set; }

    /// <summary>
    /// Total
    /// </summary>
    [Required]
    [Column("total")]
    public decimal Total { get; set; } = 0;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// ID da nota fiscal (se emitida)
    /// </summary>
    [Column("fiscal_note_id")]
    public Guid? FiscalNoteId { get; set; }

    /// <summary>
    /// Nota fiscal
    /// </summary>
    [ForeignKey("FiscalNoteId")]
    public virtual FiscalNote? FiscalNote { get; set; }

    /// <summary>
    /// Data de cancelamento
    /// </summary>
    [Column("cancelled_date")]
    public DateTime? CancelledDate { get; set; }

    /// <summary>
    /// Motivo do cancelamento
    /// </summary>
    [Column("cancelled_reason")]
    [StringLength(500)]
    public string? CancelledReason { get; set; }

    /// <summary>
    /// ID do usuário que cancelou
    /// </summary>
    [Column("cancelled_by")]
    [StringLength(450)]
    public string? CancelledBy { get; set; }

    /// <summary>
    /// Itens da compra
    /// </summary>
    public virtual ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();

    /// <summary>
    /// Contas a pagar
    /// </summary>
    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    /// <summary>
    /// Movimentações de estoque
    /// </summary>
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}

/// <summary>
/// Status da compra
/// </summary>
public enum PurchaseStatus
{
    /// <summary>
    /// Rascunho
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Confirmada
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Em aprovação
    /// </summary>
    Approving = 2,

    /// <summary>
    /// Aprovada
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Em transporte
    /// </summary>
    InTransit = 4,

    /// <summary>
    /// Recebida
    /// </summary>
    Received = 5,

    /// <summary>
    /// Faturada
    /// </summary>
    Invoiced = 6,

    /// <summary>
    /// Cancelada
    /// </summary>
    Cancelled = 7
}

/// <summary>
/// Tipo de compra
/// </summary>
public enum PurchaseType
{
    /// <summary>
    /// Compra normal
    /// </summary>
    Purchase = 0,

    /// <summary>
    /// Cotação
    /// </summary>
    Quote = 1,

    /// <summary>
    /// Pedido
    /// </summary>
    Order = 2,

    /// <summary>
    /// Devolução
    /// </summary>
    Return = 3
}

/// <summary>
/// Representa um item de compra
/// </summary>
[Table("purchase_items")]
public class PurchaseItem : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da compra
    /// </summary>
    [Required]
    [Column("purchase_id")]
    public Guid PurchaseId { get; set; }

    /// <summary>
    /// Compra
    /// </summary>
    [ForeignKey("PurchaseId")]
    public virtual Purchase Purchase { get; set; } = null!;

    /// <summary>
    /// ID do produto
    /// </summary>
    [Column("product_id")]
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Produto
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Código do produto
    /// </summary>
    [Column("product_code")]
    [StringLength(50)]
    public string? ProductCode { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    [Column("product_name")]
    [StringLength(200)]
    public string? ProductName { get; set; }

    /// <summary>
    /// Quantidade
    /// </summary>
    [Required]
    [Column("quantity")]
    public decimal Quantity { get; set; } = 0;

    /// <summary>
    /// Quantidade recebida
    /// </summary>
    [Column("received_quantity")]
    public decimal? ReceivedQuantity { get; set; }

    /// <summary>
    /// Unidade de medida
    /// </summary>
    [Column("unit_of_measure")]
    [StringLength(10)]
    public string? UnitOfMeasure { get; set; }

    /// <summary>
    /// Preço unitário
    /// </summary>
    [Required]
    [Column("unit_price")]
    public decimal UnitPrice { get; set; } = 0;

    /// <summary>
    /// Desconto
    /// </summary>
    [Column("discount")]
    public decimal? Discount { get; set; }

    /// <summary>
    /// Desconto (%)
    /// </summary>
    [Column("discount_percent")]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// ICMS (%)
    /// </summary>
    [Column("icms_rate")]
    public decimal? IcmsRate { get; set; }

    /// <summary>
    /// ICMS (valor)
    /// </summary>
    [Column("icms_value")]
    public decimal? IcmsValue { get; set; }

    /// <summary>
    /// IPI (%)
    /// </summary>
    [Column("ipi_rate")]
    public decimal? IpiRate { get; set; }

    /// <summary>
    /// IPI (valor)
    /// </summary>
    [Column("ipi_value")]
    public decimal? IpiValue { get; set; }

    /// <summary>
    /// PIS (%)
    /// </summary>
    [Column("pis_rate")]
    public decimal? PisRate { get; set; }

    /// <summary>
    /// PIS (valor)
    /// </summary>
    [Column("pis_value")]
    public decimal? PisValue { get; set; }

    /// <summary>
    /// COFINS (%)
    /// </summary>
    [Column("cofins_rate")]
    public decimal? CofinsRate { get; set; }

    /// <summary>
    /// COFINS (valor)
    /// </summary>
    [Column("cofins_value")]
    public decimal? CofinsValue { get; set; }

    /// <summary>
    /// Subtotal
    /// </summary>
    [Required]
    [Column("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    /// <summary>
    /// Total
    /// </summary>
    [Required]
    [Column("total")]
    public decimal Total { get; set; } = 0;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Movimentações de estoque
    /// </summary>
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
