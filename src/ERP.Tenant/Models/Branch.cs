using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma filial da empresa
/// </summary>
[Table("branches")]
public class Branch : BaseEntity<Guid>, ITenantEntity
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
    /// Código da filial
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome da filial
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da filial (se diferente da matriz)
    /// </summary>
    [Column("cnpj")]
    [StringLength(18)]
    public string? Cnpj { get; set; }

    /// <summary>
    /// IE da filial
    /// </summary>
    [Column("state_registration")]
    [StringLength(20)]
    public string? StateRegistration { get; set; }

    /// <summary>
    /// IM da filial
    /// </summary>
    [Column("municipal_registration")]
    [StringLength(20)]
    public string? MunicipalRegistration { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Telefone
    /// </summary>
    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// CEP
    /// </summary>
    [Column("zip_code")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// Endereço
    /// </summary>
    [Column("address")]
    [StringLength(200)]
    public string? Address { get; set; }

    /// <summary>
    /// Número
    /// </summary>
    [Column("address_number")]
    [StringLength(20)]
    public string? AddressNumber { get; set; }

    /// <summary>
    /// Complemento
    /// </summary>
    [Column("address_complement")]
    [StringLength(100)]
    public string? AddressComplement { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    [Column("neighborhood")]
    [StringLength(100)]
    public string? Neighborhood { get; set; }

    /// <summary>
    /// Cidade
    /// </summary>
    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// Estado
    /// </summary>
    [Column("state")]
    [StringLength(2)]
    public string? State { get; set; }

    /// <summary>
    /// País
    /// </summary>
    [Column("country")]
    [StringLength(100)]
    public string Country { get; set; } = "Brasil";

    /// <summary>
    /// Filial matriz
    /// </summary>
    [Required]
    [Column("is_headquarters")]
    public bool IsHeadquarters { get; set; } = false;

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Sequência de notas fiscais
    /// </summary>
    [Column("invoice_sequence")]
    public int InvoiceSequence { get; set; } = 0;

    /// <summary>
    /// Sequência de pedidos de venda
    /// </summary>
    [Column("sale_sequence")]
    public int SaleSequence { get; set; } = 0;

    /// <summary>
    /// Sequência de pedidos de compra
    /// </summary>
    [Column("purchase_sequence")]
    public int PurchaseSequence { get; set; } = 0;

    /// <summary>
    /// Estoque
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    /// <summary>
    /// Vendas
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Compras
    /// </summary>
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    /// <summary>
    /// Funcionários
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    /// <summary>
    /// Contas bancárias
    /// </summary>
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
}
