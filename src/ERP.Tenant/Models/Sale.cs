using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma venda
/// </summary>
[Table("sales")]
public class Sale : BaseEntity<Guid>, ITenantEntity
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
    /// Número da venda
    /// </summary>
    [Required]
    [Column("number")]
    [StringLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente
    /// </summary>
    [Column("client_id")]
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Cliente
    /// </summary>
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    /// <summary>
    /// Nome do cliente (se não cadastrado)
    /// </summary>
    [Column("client_name")]
    [StringLength(200)]
    public string? ClientName { get; set; }

    /// <summary>
    /// CPF/CNPJ do cliente (se não cadastrado)
    /// </summary>
    [Column("client_document")]
    [StringLength(18)]
    public string? ClientDocument { get; set; }

    /// <summary>
    /// Data da venda
    /// </summary>
    [Required]
    [Column("sale_date")]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Status da venda
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public SaleStatus Status { get; set; } = SaleStatus.Draft;

    /// <summary>
    /// Tipo de venda
    /// </summary>
    [Required]
    [Column("sale_type")]
    [StringLength(20)]
    public SaleType SaleType { get; set; } = SaleType.Sale;

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
    /// ID do vendedor
    /// </summary>
    [Column("seller_id")]
    public Guid? SellerId { get; set; }

    /// <summary>
    /// Vendedor
    /// </summary>
    [ForeignKey("SellerId")]
    public virtual Employee? Seller { get; set; }

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
    /// Itens da venda
    /// </summary>
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Contas a receber
    /// </summary>
    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();

    /// <summary>
    /// Movimentações de estoque
    /// </summary>
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}

/// <summary>
/// Status da venda
/// </summary>
public enum SaleStatus
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
    /// Em separação
    /// </summary>
    Separating = 2,

    /// <summary>
    /// Separada
    /// </summary>
    Separated = 3,

    /// <summary>
    /// Em transporte
    /// </summary>
    InTransit = 4,

    /// <summary>
    /// Entregue
    /// </summary>
    Delivered = 5,

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
/// Tipo de venda
/// </summary>
public enum SaleType
{
    /// <summary>
    /// Venda normal
    /// </summary>
    Sale = 0,

    /// <summary>
    /// Orçamento
    /// </summary>
    Quote = 1,

    /// <summary>
    /// Pedido
    /// </summary>
    Order = 2,

    /// <summary>
    /// Devolução
    /// </summary>
    Return = 3,

    /// <summary>
    /// Bonificação
    /// </summary>
    Bonus = 4
}

/// <summary>
/// Representa um item de venda
/// </summary>
[Table("sale_items")]
public class SaleItem : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da venda
    /// </summary>
    [Required]
    [Column("sale_id")]
    public Guid SaleId { get; set; }

    /// <summary>
    /// Venda
    /// </summary>
    [ForeignKey("SaleId")]
    public virtual Sale Sale { get; set; } = null!;

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
