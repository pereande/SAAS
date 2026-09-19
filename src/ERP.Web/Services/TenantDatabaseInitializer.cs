using System;
using System.Threading.Tasks;
using ERP.Shared.Settings;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ERP.Web.Services;

/// <summary>
/// Serviço responsável por criar e inicializar o banco de dados de um tenant
/// </summary>
public class TenantDatabaseInitializer
{
    private readonly DatabaseSettings _dbSettings;
    private readonly ILogger<TenantDatabaseInitializer> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="dbSettings">Configurações de banco de dados</param>
    /// <param name="logger">Logger</param>
    public TenantDatabaseInitializer(
        IOptions<DatabaseSettings> dbSettings,
        ILogger<TenantDatabaseInitializer> logger)
    {
        _dbSettings = dbSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Garante que o banco de dados do tenant exista (banco + schema + dados iniciais)
    /// </summary>
    /// <param name="tenant">Tenant</param>
    public async Task EnsureTenantDatabaseAsync(ERP.Master.Models.Tenant tenant)
    {
        await EnsureDatabaseExistsAsync(tenant.DbName);

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(tenant.ConnectionString)
            .Options;

        await using var context = new TenantDbContext(options, tenant.Id, tenant.Name);
        await context.Database.EnsureCreatedAsync();
        await SeedTenantDataAsync(context, tenant);
    }

    /// <summary>
    /// Cria o banco de dados do tenant se ainda não existir
    /// </summary>
    /// <param name="dbName">Nome do banco</param>
    private async Task EnsureDatabaseExistsAsync(string dbName)
    {
        var master = new NpgsqlConnectionStringBuilder(_dbSettings.MasterConnectionString)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(master.ConnectionString);
        await connection.OpenAsync();

        var existsCmd = new NpgsqlCommand("SELECT COUNT(1) FROM pg_database WHERE datname = @name", connection);
        existsCmd.Parameters.AddWithValue("name", dbName);
        var exists = Convert.ToInt64(await existsCmd.ExecuteScalarAsync()) > 0;

        if (exists)
        {
            return;
        }

        var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection);
        await createCmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Tenant database {DbName} created", dbName);
    }

    /// <summary>
    /// Insere os dados iniciais do tenant (empresa, filial matriz e registros de demonstração)
    /// </summary>
    /// <param name="context">DbContext do tenant</param>
    /// <param name="tenant">Tenant</param>
    private static async Task SeedTenantDataAsync(TenantDbContext context, ERP.Master.Models.Tenant tenant)
    {
        // Empresa e filial matriz
        if (!await context.Companies.AnyAsync())
        {
            var company = new Company
            {
                TenantId = tenant.Id,
                CorporateName = tenant.Name,
                TradeName = tenant.Name,
                Cnpj = tenant.Cnpj,
                Email = tenant.Email,
                Phone = tenant.Phone
            };
            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();

            var branch = new Branch
            {
                TenantId = tenant.Id,
                CompanyId = company.Id,
                Code = "001",
                Name = "Matriz",
                IsHeadquarters = true
            };
            await context.Branches.AddAsync(branch);
            await context.SaveChangesAsync();
        }

        // Pessoas de demonstração (um cliente e dois fornecedores)
        if (!await context.People.AnyAsync())
        {
            var clientPerson = new Person
            {
                TenantId = tenant.Id,
                Name = "Cliente Consumidor",
                PersonType = PersonType.Physical,
                Document = "123.456.789-00"
            };
            var supplierPerson1 = new Person
            {
                TenantId = tenant.Id,
                Name = "Fornecedor Central",
                PersonType = PersonType.Legal,
                Document = "12.345.678/0001-99"
            };
            var supplierPerson2 = new Person
            {
                TenantId = tenant.Id,
                Name = "Distribuidora Nacional",
                PersonType = PersonType.Legal,
                Document = "98.765.432/0001-11"
            };
            await context.People.AddRangeAsync(clientPerson, supplierPerson1, supplierPerson2);
            await context.SaveChangesAsync();

            await context.Clients.AddAsync(new Client
            {
                TenantId = tenant.Id,
                PersonId = clientPerson.Id,
                Code = "CL0001"
            });
            await context.Suppliers.AddRangeAsync(
                new Supplier { TenantId = tenant.Id, PersonId = supplierPerson1.Id, Code = "FR0001" },
                new Supplier { TenantId = tenant.Id, PersonId = supplierPerson2.Id, Code = "FR0002" });
            await context.SaveChangesAsync();
        }

        // Produtos de demonstração com estoque inicial
        if (!await context.Products.AnyAsync())
        {
            var branch = await context.Branches.OrderBy(b => b.Code).FirstAsync();

            var product1 = new Product
            {
                TenantId = tenant.Id, Code = "PRD-0001", Name = "Notebook Pro 15",
                Description = "Notebook profissional 15 polegadas", UnitOfMeasure = "UN",
                CostPrice = 2500.00m, SalePrice = 3499.00m, MinStock = 5, MaxStock = 100
            };
            var product2 = new Product
            {
                TenantId = tenant.Id, Code = "PRD-0002", Name = "Mouse Sem Fio",
                Description = "Mouse óptico sem fio", UnitOfMeasure = "UN",
                CostPrice = 45.00m, SalePrice = 89.90m, MinStock = 10, MaxStock = 500
            };
            var product3 = new Product
            {
                TenantId = tenant.Id, Code = "PRD-0003", Name = "Consultoria Técnica",
                Description = "Hora de consultoria técnica", UnitOfMeasure = "HR",
                ProductType = ProductType.Service, CostPrice = 80.00m, SalePrice = 150.00m,
                ManageStock = false
            };
            await context.Products.AddRangeAsync(product1, product2, product3);
            await context.SaveChangesAsync();

            await context.Inventories.AddRangeAsync(
                new Inventory
                {
                    TenantId = tenant.Id, BranchId = branch.Id, ProductId = product1.Id,
                    Quantity = 20, AverageCost = 2500.00m, Location = "A1-01"
                },
                new Inventory
                {
                    TenantId = tenant.Id, BranchId = branch.Id, ProductId = product2.Id,
                    Quantity = 150, AverageCost = 45.00m, Location = "A2-03"
                });
            await context.SaveChangesAsync();
        }
    }
}
