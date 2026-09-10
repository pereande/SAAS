using ERP.Master.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ERP.Master.Infrastructure.Data;

/// <summary>
/// Factory usada pelas ferramentas do EF Core (dotnet ef) em tempo de design
/// para gerar migrations do MasterDbContext
/// </summary>
public class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    /// <summary>
    /// Cria o contexto em tempo de design
    /// </summary>
    /// <param name="args">Argumentos</param>
    /// <returns>MasterDbContext</returns>
    public MasterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=erp_master;Username=postgres;Password=postgres");

        return new MasterDbContext(optionsBuilder.Options);
    }
}
