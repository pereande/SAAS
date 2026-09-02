using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Tenant.Models;

/// <summary>
/// Representa uma forma de pagamento
/// </summary>
[Table("payment_methods")]
public class PaymentMethod : BaseEntity<Guid>, ITenantEntity
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
    /// Código
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Tipo
    /// </summary>
    [Required]
    [Column("payment_method_type")]
    [StringLength(20)]
    public PaymentMethodType Type { get; set; } = PaymentMethodType.Cash;

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Contas a pagar
    /// </summary>
    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    /// <summary>
    /// Contas a receber
    /// </summary>
    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();

    /// <summary>
    /// Lançamentos financeiros
    /// </summary>
    public virtual ICollection<FinancialEntry> FinancialEntries { get; set; } = new List<FinancialEntry>();
}

/// <summary>
/// Tipo de forma de pagamento
/// </summary>
public enum PaymentMethodType
{
    /// <summary>
    /// Dinheiro
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Cartão de crédito
    /// </summary>
    CreditCard = 1,

    /// <summary>
    /// Cartão de débito
    /// </summary>
    DebitCard = 2,

    /// <summary>
    /// Cheque
    /// </summary>
    Check = 3,

    /// <summary>
    /// Boleto
    /// </summary>
    Boleto = 4,

    /// <summary>
    /// Transferência bancária
    /// </summary>
    BankTransfer = 5,

    /// <summary>
    /// PIX
    /// </summary>
    Pix = 6,

    /// <summary>
    /// Outro
    /// </summary>
    Other = 7
}

/// <summary>
/// Representa uma condição de pagamento
/// </summary>
[Table("payment_terms")]
public class PaymentTerm : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Dias para pagamento
    /// </summary>
    [Required]
    [Column("days")]
    public int Days { get; set; } = 0;

    /// <summary>
    /// Desconto para pagamento à vista (%)
    /// </summary>
    [Column("cash_discount_percent")]
    public decimal? CashDiscountPercent { get; set; }

    /// <summary>
    /// Desconto para pagamento à vista (valor)
    /// </summary>
    [Column("cash_discount_value")]
    public decimal? CashDiscountValue { get; set; }

    /// <summary>
    /// Multa por atraso (%)
    /// </summary>
    [Column("late_fee_percent")]
    public decimal? LateFeePercent { get; set; }

    /// <summary>
    /// Juros por dia (%)
    /// </summary>
    [Column("daily_interest_percent")]
    public decimal? DailyInterestPercent { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Vendas
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Compras
    /// </summary>
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    /// <summary>
    /// Clientes
    /// </summary>
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    /// <summary>
    /// Fornecedores
    /// </summary>
    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}

/// <summary>
/// Representa um banco
/// </summary>
[Table("banks")]
public class Bank : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Código do banco (BACEN)
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Contas bancárias
    /// </summary>
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
}

/// <summary>
/// Representa uma conta bancária
/// </summary>
[Table("bank_accounts")]
public class BankAccount : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID do banco
    /// </summary>
    [Required]
    [Column("bank_id")]
    public Guid BankId { get; set; }

    /// <summary>
    /// Banco
    /// </summary>
    [ForeignKey("BankId")]
    public virtual Bank Bank { get; set; } = null!;

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
    /// Agência
    /// </summary>
    [Column("agency")]
    [StringLength(20)]
    public string? Agency { get; set; }

    /// <summary>
    /// Dígito da agência
    /// </summary>
    [Column("agency_digit")]
    [StringLength(5)]
    public string? AgencyDigit { get; set; }

    /// <summary>
    /// Conta
    /// </summary>
    [Required]
    [Column("account")]
    [StringLength(20)]
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// Dígito da conta
    /// </summary>
    [Column("account_digit")]
    [StringLength(5)]
    public string? AccountDigit { get; set; }

    /// <summary>
    /// Tipo de conta
    /// </summary>
    [Required]
    [Column("account_type")]
    [StringLength(20)]
    public BankAccountType Type { get; set; } = BankAccountType.Checking;

    /// <summary>
    /// Nome da conta
    /// </summary>
    [Column("account_name")]
    [StringLength(100)]
    public string? AccountName { get; set; }

    /// <summary>
    /// Saldo atual
    /// </summary>
    [Column("current_balance")]
    public decimal CurrentBalance { get; set; } = 0;

    /// <summary>
    /// Saldo inicial
    /// </summary>
    [Column("initial_balance")]
    public decimal InitialBalance { get; set; } = 0;

    /// <summary>
    /// Data do saldo inicial
    /// </summary>
    [Column("initial_balance_date")]
    public DateTime? InitialBalanceDate { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Conta padrão
    /// </summary>
    [Required]
    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Lançamentos financeiros
    /// </summary>
    public virtual ICollection<FinancialEntry> FinancialEntries { get; set; } = new List<FinancialEntry>();
}

/// <summary>
/// Tipo de conta bancária
/// </summary>
public enum BankAccountType
{
    /// <summary>
    /// Conta corrente
    /// </summary>
    Checking = 0,

    /// <summary>
    /// Conta poupança
    /// </summary>
    Savings = 1,

    /// <summary>
    /// Conta salário
    /// </summary>
    Salary = 2
}

/// <summary>
/// Representa um centro de custo
/// </summary>
[Table("cost_centers")]
public class CostCenter : BaseEntity<Guid>, ITenantEntity
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
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Tipo
    /// </summary>
    [Required]
    [Column("cost_center_type")]
    [StringLength(20)]
    public CostCenterType Type { get; set; } = CostCenterType.Expense;

    /// <summary>
    /// Centro de custo pai
    /// </summary>
    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Centro de custo pai
    /// </summary>
    [ForeignKey("ParentId")]
    public virtual CostCenter? Parent { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lançamentos financeiros
    /// </summary>
    public virtual ICollection<FinancialEntry> FinancialEntries { get; set; } = new List<FinancialEntry>();
}

/// <summary>
/// Tipo de centro de custo
/// </summary>
public enum CostCenterType
{
    /// <summary>
    /// Despesa
    /// </summary>
    Expense = 0,

    /// <summary>
    /// Receita
    /// </summary>
    Revenue = 1,

    /// <summary>
    /// Custo
    /// </summary>
    Cost = 2,

    /// <summary>
    /// Investimento
    /// </summary>
    Investment = 3
}

/// <summary>
/// Representa uma conta a pagar
/// </summary>
[Table("accounts_payable")]
public class AccountPayable : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Número do título
    /// </summary>
    [Required]
    [Column("number")]
    [StringLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// ID do fornecedor
    /// </summary>
    [Column("supplier_id")]
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Fornecedor
    /// </summary>
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    /// <summary>
    /// Nome do fornecedor (se não cadastrado)
    /// </summary>
    [Column("supplier_name")]
    [StringLength(200)]
    public string? SupplierName { get; set; }

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
    /// Tipo de documento
    /// </summary>
    [Required]
    [Column("document_type")]
    [StringLength(20)]
    public AccountDocumentType DocumentType { get; set; } = AccountDocumentType.Invoice;

    /// <summary>
    /// Número do documento
    /// </summary>
    [Column("document_number")]
    [StringLength(50)]
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Data de emissão
    /// </summary>
    [Required]
    [Column("issue_date")]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de vencimento
    /// </summary>
    [Required]
    [Column("due_date")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Valor
    /// </summary>
    [Required]
    [Column("value")]
    public decimal Value { get; set; } = 0;

    /// <summary>
    /// Valor pago
    /// </summary>
    [Column("paid_value")]
    public decimal? PaidValue { get; set; }

    /// <summary>
    /// ID da forma de pagamento
    /// </summary>
    [Column("payment_method_id")]
    public Guid? PaymentMethodId { get; set; }

    /// <summary>
    /// Forma de pagamento
    /// </summary>
    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// ID do centro de custo
    /// </summary>
    [Column("cost_center_id")]
    public Guid? CostCenterId { get; set; }

    /// <summary>
    /// Centro de custo
    /// </summary>
    [ForeignKey("CostCenterId")]
    public virtual CostCenter? CostCenter { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public AccountStatus Status { get; set; } = AccountStatus.Pending;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Data de pagamento
    /// </summary>
    [Column("payment_date")]
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// ID do lançamento financeiro
    /// </summary>
    [Column("financial_entry_id")]
    public Guid? FinancialEntryId { get; set; }

    /// <summary>
    /// Lançamento financeiro
    /// </summary>
    [ForeignKey("FinancialEntryId")]
    public virtual FinancialEntry? FinancialEntry { get; set; }

    /// <summary>
    /// ID da nota fiscal (se aplicável)
    /// </summary>
    [Column("fiscal_note_id")]
    public Guid? FiscalNoteId { get; set; }

    /// <summary>
    /// Nota fiscal
    /// </summary>
    [ForeignKey("FiscalNoteId")]
    public virtual FiscalNote? FiscalNote { get; set; }
}

/// <summary>
/// Representa uma conta a receber
/// </summary>
[Table("accounts_receivable")]
public class AccountReceivable : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Número do título
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
    /// Tipo de documento
    /// </summary>
    [Required]
    [Column("document_type")]
    [StringLength(20)]
    public AccountDocumentType DocumentType { get; set; } = AccountDocumentType.Invoice;

    /// <summary>
    /// Número do documento
    /// </summary>
    [Column("document_number")]
    [StringLength(50)]
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Data de emissão
    /// </summary>
    [Required]
    [Column("issue_date")]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de vencimento
    /// </summary>
    [Required]
    [Column("due_date")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Valor
    /// </summary>
    [Required]
    [Column("value")]
    public decimal Value { get; set; } = 0;

    /// <summary>
    /// Valor recebido
    /// </summary>
    [Column("received_value")]
    public decimal? ReceivedValue { get; set; }

    /// <summary>
    /// ID da forma de pagamento
    /// </summary>
    [Column("payment_method_id")]
    public Guid? PaymentMethodId { get; set; }

    /// <summary>
    /// Forma de pagamento
    /// </summary>
    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// ID do centro de custo
    /// </summary>
    [Column("cost_center_id")]
    public Guid? CostCenterId { get; set; }

    /// <summary>
    /// Centro de custo
    /// </summary>
    [ForeignKey("CostCenterId")]
    public virtual CostCenter? CostCenter { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public AccountStatus Status { get; set; } = AccountStatus.Pending;

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Data de recebimento
    /// </summary>
    [Column("receipt_date")]
    public DateTime? ReceiptDate { get; set; }

    /// <summary>
    /// ID do lançamento financeiro
    /// </summary>
    [Column("financial_entry_id")]
    public Guid? FinancialEntryId { get; set; }

    /// <summary>
    /// Lançamento financeiro
    /// </summary>
    [ForeignKey("FinancialEntryId")]
    public virtual FinancialEntry? FinancialEntry { get; set; }

    /// <summary>
    /// ID da nota fiscal (se aplicável)
    /// </summary>
    [Column("fiscal_note_id")]
    public Guid? FiscalNoteId { get; set; }

    /// <summary>
    /// Nota fiscal
    /// </summary>
    [ForeignKey("FiscalNoteId")]
    public virtual FiscalNote? FiscalNote { get; set; }
}

/// <summary>
/// Tipo de documento
/// </summary>
public enum AccountDocumentType
{
    /// <summary>
    /// Nota fiscal
    /// </summary>
    Invoice = 0,

    /// <summary>
    /// Duplicata
    /// </summary>
    Duplicate = 1,

    /// <summary>
    /// Boleto
    /// </summary>
    Boleto = 2,

    /// <summary>
    /// Recibo
    /// </summary>
    Receipt = 3,

    /// <summary>
    /// Nota fiscal de serviço
    /// </summary>
    ServiceInvoice = 4
}

/// <summary>
/// Status da conta
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Pendente
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Pago/Recebido
    /// </summary>
    Paid = 1,

    /// <summary>
    /// Parcialmente pago
    /// </summary>
    PartiallyPaid = 2,

    /// <summary>
    /// Vencido
    /// </summary>
    Overdue = 3,

    /// <summary>
    /// Cancelado
    /// </summary>
    Cancelled = 4
}

/// <summary>
/// Representa um lançamento financeiro
/// </summary>
[Table("financial_entries")]
public class FinancialEntry : BaseEntity<Guid>, ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Número do lançamento
    /// </summary>
    [Required]
    [Column("number")]
    [StringLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Data do lançamento
    /// </summary>
    [Required]
    [Column("entry_date")]
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tipo
    /// </summary>
    [Required]
    [Column("entry_type")]
    [StringLength(20)]
    public FinancialEntryType Type { get; set; } = FinancialEntryType.Expense;

    /// <summary>
    /// Descrição
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor
    /// </summary>
    [Required]
    [Column("value")]
    public decimal Value { get; set; } = 0;

    /// <summary>
    /// ID da forma de pagamento
    /// </summary>
    [Column("payment_method_id")]
    public Guid? PaymentMethodId { get; set; }

    /// <summary>
    /// Forma de pagamento
    /// </summary>
    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// ID do centro de custo
    /// </summary>
    [Column("cost_center_id")]
    public Guid? CostCenterId { get; set; }

    /// <summary>
    /// Centro de custo
    /// </summary>
    [ForeignKey("CostCenterId")]
    public virtual CostCenter? CostCenter { get; set; }

    /// <summary>
    /// ID da conta bancária
    /// </summary>
    [Column("bank_account_id")]
    public Guid? BankAccountId { get; set; }

    /// <summary>
    /// Conta bancária
    /// </summary>
    [ForeignKey("BankAccountId")]
    public virtual BankAccount? BankAccount { get; set; }

    /// <summary>
    /// ID do fornecedor (para despesas)
    /// </summary>
    [Column("supplier_id")]
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Fornecedor
    /// </summary>
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    /// <summary>
    /// ID do cliente (para receitas)
    /// </summary>
    [Column("client_id")]
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Cliente
    /// </summary>
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    /// <summary>
    /// Número do documento
    /// </summary>
    [Column("document_number")]
    [StringLength(50)]
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Data do documento
    /// </summary>
    [Column("document_date")]
    public DateTime? DocumentDate { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public FinancialEntryStatus Status { get; set; } = FinancialEntryStatus.Pending;

    /// <summary>
    /// Conciliado
    /// </summary>
    [Required]
    [Column("is_reconciled")]
    public bool IsReconciled { get; set; } = false;

    /// <summary>
    /// Data de conciliação
    /// </summary>
    [Column("reconciliation_date")]
    public DateTime? ReconciliationDate { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    [Column("notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Contas a pagar
    /// </summary>
    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    /// <summary>
    /// Contas a receber
    /// </summary>
    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();
}

/// <summary>
/// Tipo de lançamento financeiro
/// </summary>
public enum FinancialEntryType
{
    /// <summary>
    /// Despesa
    /// </summary>
    Expense = 0,

    /// <summary>
    /// Receita
    /// </summary>
    Revenue = 1,

    /// <summary>
    /// Transferência entre contas
    /// </summary>
    Transfer = 2
}

/// <summary>
/// Status do lançamento financeiro
/// </summary>
public enum FinancialEntryStatus
{
    /// <summary>
    /// Pendente
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Confirmado
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Cancelado
    /// </summary>
    Cancelled = 2
}
