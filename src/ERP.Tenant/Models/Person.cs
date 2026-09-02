using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma pessoa (base para clientes, fornecedores, funcionários)
/// </summary>
[Table("people")]
public class Person : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Tipo de pessoa
    /// </summary>
    [Required]
    [Column("person_type")]
    [StringLength(20)]
    public PersonType PersonType { get; set; } = PersonType.Physical;

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CPF/CNPJ
    /// </summary>
    [Column("document")]
    [StringLength(18)]
    public string? Document { get; set; }

    /// <summary>
    /// RG/IE
    /// </summary>
    [Column("rg_ie")]
    [StringLength(20)]
    public string? RgIe { get; set; }

    /// <summary>
    /// Data de nascimento/fundação
    /// </summary>
    [Column("birth_date")]
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Telefone principal
    /// </summary>
    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Telefone secundário
    /// </summary>
    [Column("secondary_phone")]
    [StringLength(20)]
    public string? SecondaryPhone { get; set; }

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
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Tipo de pessoa
/// </summary>
public enum PersonType
{
    /// <summary>
    /// Pessoa Física
    /// </summary>
    Physical = 0,

    /// <summary>
    /// Pessoa Jurídica
    /// </summary>
    Legal = 1
}

/// <summary>
/// Representa um cliente
/// </summary>
[Table("clients")]
public class Client : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da pessoa
    /// </summary>
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Pessoa
    /// </summary>
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; } = null!;

    /// <summary>
    /// Código do cliente
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Limite de crédito
    /// </summary>
    [Column("credit_limit")]
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Dias de prazo para pagamento
    /// </summary>
    [Column("payment_days")]
    public int? PaymentDays { get; set; }

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
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Vendas
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Contas a receber
    /// </summary>
    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();
}

/// <summary>
/// Representa um fornecedor
/// </summary>
[Table("suppliers")]
public class Supplier : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da pessoa
    /// </summary>
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Pessoa
    /// </summary>
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; } = null!;

    /// <summary>
    /// Código do fornecedor
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Contato
    /// </summary>
    [Column("contact")]
    [StringLength(200)]
    public string? Contact { get; set; }

    /// <summary>
    /// Telefone de contato
    /// </summary>
    [Column("contact_phone")]
    [StringLength(20)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// E-mail de contato
    /// </summary>
    [Column("contact_email")]
    [StringLength(255)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Compras
    /// </summary>
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    /// <summary>
    /// Produtos fornecidos
    /// </summary>
    public virtual ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();

    /// <summary>
    /// Contas a pagar
    /// </summary>
    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();
}

/// <summary>
/// Representa um funcionário
/// </summary>
[Table("employees")]
public class Employee : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID da pessoa
    /// </summary>
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Pessoa
    /// </summary>
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; } = null!;

    /// <summary>
    /// ID da filial
    /// </summary>
    [Column("branch_id")]
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Filial
    /// </summary>
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }

    /// <summary>
    /// Matrícula
    /// </summary>
    [Required]
    [Column("registration")]
    [StringLength(20)]
    public string Registration { get; set; } = string.Empty;

    /// <summary>
    /// Cargo
    /// </summary>
    [Column("position")]
    [StringLength(100)]
    public string? Position { get; set; }

    /// <summary>
    /// Departamento
    /// </summary>
    [Column("department")]
    [StringLength(100)]
    public string? Department { get; set; }

    /// <summary>
    /// Salário
    /// </summary>
    [Column("salary")]
    public decimal? Salary { get; set; }

    /// <summary>
    /// Data de admissão
    /// </summary>
    [Column("hire_date")]
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Data de demissão
    /// </summary>
    [Column("termination_date")]
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// Tipo de contratação
    /// </summary>
    [Column("employment_type")]
    [StringLength(50)]
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }
}
