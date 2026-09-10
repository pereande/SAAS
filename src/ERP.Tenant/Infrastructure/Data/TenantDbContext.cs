using System;
using System.Reflection;
using ERP.Shared.Models;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tenant.Infrastructure.Data;

/// <summary>
/// DbContext para os bancos de dados dos tenants (ERP_EMPRESA_XXX)
/// Cada tenant tem seu próprio banco de dados com estas tabelas
/// </summary>
public class TenantDbContext : DbContext
{
    /// <summary>
    /// ID do tenant atual
    /// </summary>
    public Guid TenantId { get; }

    /// <summary>
    /// Nome do tenant
    /// </summary>
    public string TenantName { get; }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="options">Opções do DbContext</param>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="tenantName">Nome do tenant</param>
    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        Guid tenantId,
        string tenantName)
        : base(options)
    {
        TenantId = tenantId;
        TenantName = tenantName;
    }

    /// <summary>
    /// Empresas (dados da empresa do tenant)
    /// </summary>
    public DbSet<Company> Companies { get; set; } = null!;

    /// <summary>
    /// Filiais
    /// </summary>
    public DbSet<Branch> Branches { get; set; } = null!;

    /// <summary>
    /// Pessoas (clientes, fornecedores, funcionários)
    /// </summary>
    public DbSet<Person> People { get; set; } = null!;

    /// <summary>
    /// Clientes
    /// </summary>
    public DbSet<Client> Clients { get; set; } = null!;

    /// <summary>
    /// Fornecedores
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; } = null!;

    /// <summary>
    /// Funcionários
    /// </summary>
    public DbSet<Employee> Employees { get; set; } = null!;

    /// <summary>
    /// Categorias de produtos
    /// </summary>
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;

    /// <summary>
    /// Marcas
    /// </summary>
    public DbSet<Brand> Brands { get; set; } = null!;

    /// <summary>
    /// Produtos
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Estoque
    /// </summary>
    public DbSet<Inventory> Inventories { get; set; } = null!;

    /// <summary>
    /// Movimentações de estoque
    /// </summary>
    public DbSet<InventoryMovement> InventoryMovements { get; set; } = null!;

    /// <summary>
    /// Vendas
    /// </summary>
    public DbSet<Sale> Sales { get; set; } = null!;

    /// <summary>
    /// Itens de venda
    /// </summary>
    public DbSet<SaleItem> SaleItems { get; set; } = null!;

    /// <summary>
    /// Compras
    /// </summary>
    public DbSet<Purchase> Purchases { get; set; } = null!;

    /// <summary>
    /// Itens de compra
    /// </summary>
    public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;

    /// <summary>
    /// Contas a pagar
    /// </summary>
    public DbSet<AccountPayable> AccountsPayable { get; set; } = null!;

    /// <summary>
    /// Contas a receber
    /// </summary>
    public DbSet<AccountReceivable> AccountsReceivable { get; set; } = null!;

    /// <summary>
    /// Formas de pagamento
    /// </summary>
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;

    /// <summary>
    /// Condições de pagamento
    /// </summary>
    public DbSet<PaymentTerm> PaymentTerms { get; set; } = null!;

    /// <summary>
    /// Bancos
    /// </summary>
    public DbSet<Bank> Banks { get; set; } = null!;

    /// <summary>
    /// Contas bancárias
    /// </summary>
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;

    /// <summary>
    /// Lançamentos financeiros
    /// </summary>
    public DbSet<FinancialEntry> FinancialEntries { get; set; } = null!;

    /// <summary>
    /// Centros de custo
    /// </summary>
    public DbSet<CostCenter> CostCenters { get; set; } = null!;

    /// <summary>
    /// Notas fiscais de saída
    /// </summary>
    public DbSet<FiscalNote> FiscalNotes { get; set; } = null!;

    /// <summary>
    /// Itens de nota fiscal
    /// </summary>
    public DbSet<FiscalNoteItem> FiscalNoteItems { get; set; } = null!;

    /// <summary>
    /// CFOP
    /// </summary>
    public DbSet<Cfop> Cfops { get; set; } = null!;

    /// <summary>
    /// NCM
    /// </summary>
    public DbSet<Ncm> Ncms { get; set; } = null!;

    /// <summary>
    /// CST (ICMS)
    /// </summary>
    public DbSet<Cst> Csts { get; set; } = null!;

    /// <summary>
    /// CSOSN (ICMS)
    /// </summary>
    public DbSet<Csosn> Csosns { get; set; } = null!;

    /// <summary>
    /// Configuração da entidade
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações de todas as entidades no assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configurações específicas para o schema
        ConfigureSchema(modelBuilder);

        // Adicionar filtro global para tenant
        AddTenantFilter(modelBuilder);
    }

    /// <summary>
    /// Configura o schema do banco
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    private static void ConfigureSchema(ModelBuilder modelBuilder)
    {
        // Definir schema padrão
        modelBuilder.HasDefaultSchema("public");
    }

    /// <summary>
    /// Adiciona filtro global para tenant
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    private void AddTenantFilter(ModelBuilder modelBuilder)
    {
        // Obter todas as entidades que implementam ITenantEntity
        var tenantEntities = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ITenantEntity).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var entityType in tenantEntities)
        {
            // Adicionar query filter para tenant_id
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var tenantIdProperty = entityType.FindProperty("TenantId");
            
            if (tenantIdProperty != null)
            {
                var tenantIdAccess = Expression.Property(parameter, tenantIdProperty.PropertyInfo);
                var tenantIdConstant = Expression.Constant(TenantId);
                var tenantIdEqual = Expression.Equal(tenantIdAccess, tenantIdConstant);
                
                var lambda = Expression.Lambda(tenantIdEqual, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Configurações adicionais
    /// </summary>
    /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configurar para usar snake_case no PostgreSQL
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}


