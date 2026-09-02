using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma categoria de produto
/// </summary>
[Table("product_categories")]
public class ProductCategory : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Categoria pai
    /// </summary>
    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Categoria pai
    /// </summary>
    [ForeignKey("ParentId")]
    public virtual ProductCategory? Parent { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    [Column("code")]
    [StringLength(20)]
    public string? Code { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ordem de exibição
    /// </summary>
    [Column("display_order")]
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Subcategorias
    /// </summary>
    public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();

    /// <summary>
    /// Produtos
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Representa uma marca
/// </summary>
[Table("brands")]
public class Brand : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Site
    /// </summary>
    [Column("website")]
    [StringLength(500)]
    public string? Website { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Produtos
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Representa um produto
/// </summary>
[Table("products")]
public class Product : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Código de barras
    /// </summary>
    [Column("barcode")]
    [StringLength(100)]
    public string? Barcode { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// ID da categoria
    /// </summary>
    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Categoria
    /// </summary>
    [ForeignKey("CategoryId")]
    public virtual ProductCategory? Category { get; set; }

    /// <summary>
    /// ID da marca
    /// </summary>
    [Column("brand_id")]
    public Guid? BrandId { get; set; }

    /// <summary>
    /// Marca
    /// </summary>
    [ForeignKey("BrandId")]
    public virtual Brand? Brand { get; set; }

    /// <summary>
    /// Tipo do produto
    /// </summary>
    [Required]
    [Column("product_type")]
    [StringLength(20)]
    public ProductType ProductType { get; set; } = ProductType.Product;

    /// <summary>
    /// Unidade de medida
    /// </summary>
    [Required]
    [Column("unit_of_measure")]
    [StringLength(10)]
    public string UnitOfMeasure { get; set; } = "UN";

    /// <summary>
    /// Preço de custo
    /// </summary>
    [Column("cost_price")]
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// Preço de venda
    /// </summary>
    [Column("sale_price")]
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Preço de venda mínimo
    /// </summary>
    [Column("min_sale_price")]
    public decimal? MinSalePrice { get; set; }

    /// <summary>
    /// Margem de lucro (%)
    /// </summary>
    [Column("profit_margin")]
    public decimal? ProfitMargin { get; set; }

    /// <summary>
    /// ICMS (%)
    /// </summary>
    [Column("icms_rate")]
    public decimal? IcmsRate { get; set; }

    /// <summary>
    /// IPI (%)
    /// </summary>
    [Column("ipi_rate")]
    public decimal? IpiRate { get; set; }

    /// <summary>
    /// PIS (%)
    /// </summary>
    [Column("pis_rate")]
    public decimal? PisRate { get; set; }

    /// <summary>
    /// COFINS (%)
    /// </summary>
    [Column("cofins_rate")]
    public decimal? CofinsRate { get; set; }

    /// <summary>
    /// ID do NCM
    /// </summary>
    [Column("ncm_id")]
    public Guid? NcmId { get; set; }

    /// <summary>
    /// NCM
    /// </summary>
    [ForeignKey("NcmId")]
    public virtual Ncm? Ncm { get; set; }

    /// <summary>
    /// ID do CST (ICMS)
    /// </summary>
    [Column("cst_id")]
    public Guid? CstId { get; set; }

    /// <summary>
    /// CST (ICMS)
    /// </summary>
    [ForeignKey("CstId")]
    public virtual Cst? Cst { get; set; }

    /// <summary>
    /// ID do CSOSN (ICMS)
    /// </summary>
    [Column("csosn_id")]
    public Guid? CsosnId { get; set; }

    /// <summary>
    /// CSOSN (ICMS)
    /// </summary>
    [ForeignKey("CsosnId")]
    public virtual Csosn? Csosn { get; set; }

    /// <summary>
    /// Estoque mínimo
    /// </summary>
    [Column("min_stock")]
    public decimal? MinStock { get; set; }

    /// <summary>
    /// Estoque máximo
    /// </summary>
    [Column("max_stock")]
    public decimal? MaxStock { get; set; }

    /// <summary>
    /// Ponto de reposição
    /// </summary>
    [Column("reorder_point")]
    public decimal? ReorderPoint { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Controla estoque
    /// </summary>
    [Required]
    [Column("manage_stock")]
    public bool ManageStock { get; set; } = true;

    /// <summary>
    /// Permite venda sem estoque
    /// </summary>
    [Required]
    [Column("allow_sale_without_stock")]
    public bool AllowSaleWithoutStock { get; set; } = false;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Estoque
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    /// <summary>
    /// Itens de venda
    /// </summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Itens de compra
    /// </summary>
    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    /// <summary>
    /// Produtos por fornecedor
    /// </summary>
    public virtual ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
}

/// <summary>
/// Tipo de produto
/// </summary>
public enum ProductType
{
    /// <summary>
    /// Produto
    /// </summary>
    Product = 0,

    /// <summary>
    /// Serviço
    /// </summary>
    Service = 1,

    /// <summary>
    /// Combo
    /// </summary>
    Combo = 2,

    /// <summary>
    /// Kit
    /// </summary>
    Kit = 3
}

/// <summary>
/// Representa o relacionamento entre produto e fornecedor
/// </summary>
[Table("supplier_products")]
public class SupplierProduct : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

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
    /// Código do fornecedor para o produto
    /// </summary>
    [Column("supplier_code")]
    [StringLength(50)]
    public string? SupplierCode { get; set; }

    /// <summary>
    /// Preço de compra
    /// </summary>
    [Column("purchase_price")]
    public decimal? PurchasePrice { get; set; }

    /// <summary>
    /// Prazo de entrega (dias)
    /// </summary>
    [Column("delivery_days")]
    public int? DeliveryDays { get; set; }

    /// <summary>
    /// Quantidade mínima de compra
    /// </summary>
    [Column("min_purchase_quantity")]
    public decimal? MinPurchaseQuantity { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
