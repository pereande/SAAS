using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa os dados da empresa do tenant
/// </summary>
[Table("companies")]
public class Company : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant (chave estrangeira)
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Razão social
    /// </summary>
    [Required]
    [Column("corporate_name")]
    [StringLength(200)]
    public string CorporateName { get; set; } = string.Empty;

    /// <summary>
    /// Nome fantasia
    /// </summary>
    [Column("trade_name")]
    [StringLength(200)]
    public string? TradeName { get; set; }

    /// <summary>
    /// CNPJ
    /// </summary>
    [Required]
    [Column("cnpj")]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// IE (Inscrição Estadual)
    /// </summary>
    [Column("state_registration")]
    [StringLength(20)]
    public string? StateRegistration { get; set; }

    /// <summary>
    /// IM (Inscrição Municipal)
    /// </summary>
    [Column("municipal_registration")]
    [StringLength(20)]
    public string? MunicipalRegistration { get; set; }

    /// <summary>
    /// Regime tributário
    /// </summary>
    [Required]
    [Column("tax_regime")]
    [StringLength(20)]
    public TaxRegime TaxRegime { get; set; } = TaxRegime.SimpleNational;

    /// <summary>
    /// Data de abertura
    /// </summary>
    [Column("opening_date")]
    public DateTime? OpeningDate { get; set; }

    /// <summary>
    /// E-mail principal
    /// </summary>
    [Required]
    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone principal
    /// </summary>
    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Site
    /// </summary>
    [Column("website")]
    [StringLength(500)]
    public string? Website { get; set; }

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
    /// Logo URL
    /// </summary>
    [Column("logo_url")]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Filiais
    /// </summary>
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    /// <summary>
    /// Configurações fiscais
    /// </summary>
    public virtual ICollection<FiscalConfiguration> FiscalConfigurations { get; set; } = new List<FiscalConfiguration>();
}

/// <summary>
/// Regimes tributários
/// </summary>
public enum TaxRegime
{
    /// <summary>
    /// Simples Nacional
    /// </summary>
    SimpleNational = 0,

    /// <summary>
    /// Lucro Presumido
    /// </summary>
    PresumedProfit = 1,

    /// <summary>
    /// Lucro Real
    /// </summary>
    RealProfit = 2,

    /// <summary>
    /// MEI (Microempreendedor Individual)
    /// </summary>
    MEI = 3
}
