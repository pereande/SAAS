using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma nota fiscal
/// </summary>
[Table("fiscal_notes")]
public class FiscalNote : BaseEntity<Guid>, ITenantEntity
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
    /// Tipo de nota fiscal
    /// </summary>
    [Required]
    [Column("note_type")]
    [StringLength(20)]
    public FiscalNoteType NoteType { get; set; } = FiscalNoteType.Output;

    /// <summary>
    /// Série
    /// </summary>
    [Required]
    [Column("series")]
    [StringLength(10)]
    public string Series { get; set; } = "1";

    /// <summary>
    /// Número
    /// </summary>
    [Required]
    [Column("number")]
    [StringLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Data de emissão
    /// </summary>
    [Required]
    [Column("issue_date")]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de saída/entrada
    /// </summary>
    [Column("entry_exit_date")]
    public DateTime? EntryExitDate { get; set; }

    /// <summary>
    /// ID do cliente (para notas de saída)
    /// </summary>
    [Column("client_id")]
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Cliente
    /// </summary>
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    /// <summary>
    /// Nome do cliente
    /// </summary>
    [Column("client_name")]
    [StringLength(200)]
    public string? ClientName { get; set; }

    /// <summary>
    /// CPF/CNPJ do cliente
    /// </summary>
    [Column("client_document")]
    [StringLength(18)]
    public string? ClientDocument { get; set; }

    /// <summary>
    /// IE do cliente
    /// </summary>
    [Column("client_ie")]
    [StringLength(20)]
    public string? ClientIe { get; set; }

    /// <summary>
    /// ID do fornecedor (para notas de entrada)
    /// </summary>
    [Column("supplier_id")]
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Fornecedor
    /// </summary>
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    /// <summary>
    /// Nome do fornecedor
    /// </summary>
    [Column("supplier_name")]
    [StringLength(200)]
    public string? SupplierName { get; set; }

    /// <summary>
    /// CPF/CNPJ do fornecedor
    /// </summary>
    [Column("supplier_document")]
    [StringLength(18)]
    public string? SupplierDocument { get; set; }

    /// <summary>
    /// IE do fornecedor
    /// </summary>
    [Column("supplier_ie")]
    [StringLength(20)]
    public string? SupplierIe { get; set; }

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
    /// CFOP
    /// </summary>
    [Column("cfop")]
    [StringLength(10)]
    public string? Cfop { get; set; }

    /// <summary>
    /// Base de cálculo ICMS
    /// </summary>
    [Column("icms_basis")]
    public decimal? IcmsBasis { get; set; }

    /// <summary>
    /// Valor ICMS
    /// </summary>
    [Column("icms_value")]
    public decimal? IcmsValue { get; set; }

    /// <summary>
    /// Alíquota ICMS
    /// </summary>
    [Column("icms_rate")]
    public decimal? IcmsRate { get; set; }

    /// <summary>
    /// Base de cálculo IPI
    /// </summary>
    [Column("ipi_basis")]
    public decimal? IpiBasis { get; set; }

    /// <summary>
    /// Valor IPI
    /// </summary>
    [Column("ipi_value")]
    public decimal? IpiValue { get; set; }

    /// <summary>
    /// Alíquota IPI
    /// </summary>
    [Column("ipi_rate")]
    public decimal? IpiRate { get; set; }

    /// <summary>
    /// Base de cálculo PIS
    /// </summary>
    [Column("pis_basis")]
    public decimal? PisBasis { get; set; }

    /// <summary>
    /// Valor PIS
    /// </summary>
    [Column("pis_value")]
    public decimal? PisValue { get; set; }

    /// <summary>
    /// Alíquota PIS
    /// </summary>
    [Column("pis_rate")]
    public decimal? PisRate { get; set; }

    /// <summary>
    /// Base de cálculo COFINS
    /// </summary>
    [Column("cofins_basis")]
    public decimal? CofinsBasis { get; set; }

    /// <summary>
    /// Valor COFINS
    /// </summary>
    [Column("cofins_value")]
    public decimal? CofinsValue { get; set; }

    /// <summary>
    /// Alíquota COFINS
    /// </summary>
    [Column("cofins_rate")]
    public decimal? CofinsRate { get; set; }

    /// <summary>
    /// Subtotal
    /// </summary>
    [Required]
    [Column("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    /// <summary>
    /// Desconto
    /// </summary>
    [Column("discount")]
    public decimal? Discount { get; set; }

    /// <summary>
    /// Frete
    /// </summary>
    [Column("freight")]
    public decimal? Freight { get; set; }

    /// <summary>
    /// Seguro
    /// </summary>
    [Column("insurance")]
    public decimal? Insurance { get; set; }

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
    /// Status
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public FiscalNoteStatus Status { get; set; } = FiscalNoteStatus.Pending;

    /// <summary>
    /// Autorizada
    /// </summary>
    [Required]
    [Column("is_authorized")]
    public bool IsAuthorized { get; set; } = false;

    /// <summary>
    /// Data de autorização
    /// </summary>
    [Column("authorization_date")]
    public DateTime? AuthorizationDate { get; set; }

    /// <summary>
    /// Número do protocolo de autorização
    /// </summary>
    [Column("authorization_protocol")]
    [StringLength(50)]
    public string? AuthorizationProtocol { get; set; }

    /// <summary>
    /// XML
    /// </summary>
    [Column("xml")]
    public string? Xml { get; set; }

    /// <summary>
    /// PDF
    /// </summary>
    [Column("pdf")]
    public byte[]? Pdf { get; set; }

    /// <summary>
    /// Chave de acesso
    /// </summary>
    [Column("access_key")]
    [StringLength(50)]
    public string? AccessKey { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Itens da nota fiscal
    /// </summary>
    public virtual ICollection<FiscalNoteItem> Items { get; set; } = new List<FiscalNoteItem>();

    /// <summary>
    /// Contas a pagar (para notas de entrada)
    /// </summary>
    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    /// <summary>
    /// Contas a receber (para notas de saída)
    /// </summary>
    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();
}

/// <summary>
/// Tipo de nota fiscal
/// </summary>
public enum FiscalNoteType
{
    /// <summary>
    /// Nota fiscal de saída
    /// </summary>
    Output = 0,

    /// <summary>
    /// Nota fiscal de entrada
    /// </summary>
    Input = 1,

    /// <summary>
    /// Nota fiscal de serviço
    /// </summary>
    Service = 2,

    /// <summary>
    /// Nota fiscal de consumidor
    /// </summary>
    Consumer = 3
}

/// <summary>
/// Status da nota fiscal
/// </summary>
public enum FiscalNoteStatus
{
    /// <summary>
    /// Pendente
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Autorizada
    /// </summary>
    Authorized = 1,

    /// <summary>
    /// Cancelada
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Denegada
    /// </summary>
    Denied = 3
}

/// <summary>
/// Representa um item de nota fiscal
/// </summary>
[Table("fiscal_note_items")]
public class FiscalNoteItem : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da nota fiscal
    /// </summary>
    [Required]
    [Column("fiscal_note_id")]
    public Guid FiscalNoteId { get; set; }

    /// <summary>
    /// Nota fiscal
    /// </summary>
    [ForeignKey("FiscalNoteId")]
    public virtual FiscalNote FiscalNote { get; set; } = null!;

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
    /// NCM
    /// </summary>
    [Column("ncm")]
    [StringLength(20)]
    public string? Ncm { get; set; }

    /// <summary>
    /// CFOP
    /// </summary>
    [Column("cfop")]
    [StringLength(10)]
    public string? Cfop { get; set; }

    /// <summary>
    /// CST (ICMS)
    /// </summary>
    [Column("cst")]
    [StringLength(10)]
    public string? Cst { get; set; }

    /// <summary>
    /// CSOSN (ICMS)
    /// </summary>
    [Column("csosn")]
    [StringLength(10)]
    public string? Csosn { get; set; }

    /// <summary>
    /// Unidade de medida
    /// </summary>
    [Column("unit_of_measure")]
    [StringLength(10)]
    public string? UnitOfMeasure { get; set; }

    /// <summary>
    /// Quantidade
    /// </summary>
    [Required]
    [Column("quantity")]
    public decimal Quantity { get; set; } = 0;

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
    /// Total
    /// </summary>
    [Required]
    [Column("total")]
    public decimal Total { get; set; } = 0;

    /// <summary>
    /// Base de cálculo ICMS
    /// </summary>
    [Column("icms_basis")]
    public decimal? IcmsBasis { get; set; }

    /// <summary>
    /// Valor ICMS
    /// </summary>
    [Column("icms_value")]
    public decimal? IcmsValue { get; set; }

    /// <summary>
    /// Alíquota ICMS
    /// </summary>
    [Column("icms_rate")]
    public decimal? IcmsRate { get; set; }

    /// <summary>
    /// Base de cálculo IPI
    /// </summary>
    [Column("ipi_basis")]
    public decimal? IpiBasis { get; set; }

    /// <summary>
    /// Valor IPI
    /// </summary>
    [Column("ipi_value")]
    public decimal? IpiValue { get; set; }

    /// <summary>
    /// Alíquota IPI
    /// </summary>
    [Column("ipi_rate")]
    public decimal? IpiRate { get; set; }

    /// <summary>
    /// Base de cálculo PIS
    /// </summary>
    [Column("pis_basis")]
    public decimal? PisBasis { get; set; }

    /// <summary>
    /// Valor PIS
    /// </summary>
    [Column("pis_value")]
    public decimal? PisValue { get; set; }

    /// <summary>
    /// Alíquota PIS
    /// </summary>
    [Column("pis_rate")]
    public decimal? PisRate { get; set; }

    /// <summary>
    /// Base de cálculo COFINS
    /// </summary>
    [Column("cofins_basis")]
    public decimal? CofinsBasis { get; set; }

    /// <summary>
    /// Valor COFINS
    /// </summary>
    [Column("cofins_value")]
    public decimal? CofinsValue { get; set; }

    /// <summary>
    /// Alíquota COFINS
    /// </summary>
    [Column("cofins_rate")]
    public decimal? CofinsRate { get; set; }
}

/// <summary>
/// Representa um CFOP
/// </summary>
[Table("cfops")]
public class Cfop : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Aplicação
    /// </summary>
    [Column("application")]
    [StringLength(100)]
    public string? Application { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Representa um NCM
/// </summary>
[Table("ncms")]
public class Ncm : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Alíquota de IPI
    /// </summary>
    [Column("ipi_rate")]
    public decimal? IpiRate { get; set; }

    /// <summary>
    /// Ativo
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
/// Representa um CST (ICMS)
/// </summary>
[Table("csts")]
public class Cst : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
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
/// Representa um CSOSN (ICMS)
/// </summary>
[Table("csosns")]
public class Csosn : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
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
/// Representa uma configuração fiscal
/// </summary>
[Table("fiscal_configurations")]
public class FiscalConfiguration : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da empresa
    /// </summary>
    [Required]
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Empresa
    /// </summary>
    [ForeignKey("CompanyId")]
    public virtual Company Company { get; set; } = null!;

    /// <summary>
    /// CFOP padrão de saída
    /// </summary>
    [Column("default_output_cfop")]
    [StringLength(10)]
    public string? DefaultOutputCfop { get; set; }

    /// <summary>
    /// CFOP padrão de entrada
    /// </summary>
    [Column("default_input_cfop")]
    [StringLength(10)]
    public string? DefaultInputCfop { get; set; }

    /// <summary>
    /// CST padrão (ICMS)
    /// </summary>
    [Column("default_cst")]
    [StringLength(10)]
    public string? DefaultCst { get; set; }

    /// <summary>
    /// CSOSN padrão (ICMS)
    /// </summary>
    [Column("default_csosn")]
    [StringLength(10)]
    public string? DefaultCsosn { get; set; }

    /// <summary>
    /// Alíquota padrão de ICMS
    /// </summary>
    [Column("default_icms_rate")]
    public decimal? DefaultIcmsRate { get; set; }

    /// <summary>
    /// Emitir nota fiscal automaticamente
    /// </summary>
    [Required]
    [Column("auto_issue_fiscal_note")]
    public bool AutoIssueFiscalNote { get; set; } = false;

    /// <summary>
    /// Série padrão de notas fiscais
    /// </summary>
    [Column("default_series")]
    [StringLength(10)]
    public string DefaultSeries { get; set; } = "1";

    /// <summary>
    /// Próximo número de nota fiscal
    /// </summary>
    [Column("next_fiscal_note_number")]
    public int NextFiscalNoteNumber { get; set; } = 1;

    /// <summary>
    /// Ambiente (produção ou homologação)
    /// </summary>
    [Required]
    [Column("environment")]
    [StringLength(20)]
    public FiscalEnvironment Environment { get; set; } = FiscalEnvironment.Production;

    /// <summary>
    /// Certificado digital
    /// </summary>
    [Column("digital_certificate")]
    public byte[]? DigitalCertificate { get; set; }

    /// <summary>
    /// Senha do certificado
    /// </summary>
    [Column("certificate_password")]
    [StringLength(100)]
    public string? CertificatePassword { get; set; }

    /// <summary>
    /// Data de validade do certificado
    /// </summary>
    [Column("certificate_expiry_date")]
    public DateTime? CertificateExpiryDate { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Ambiente fiscal
/// </summary>
public enum FiscalEnvironment
{
    /// <summary>
    /// Produção
    /// </summary>
    Production = 0,

    /// <summary>
    /// Homologação
    /// </summary>
    Homologation = 1
}
