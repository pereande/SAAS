using System;
using System.Threading.Tasks;
using ERP.Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Services;

/// <summary>
/// Seeder idempotente da tabela formal de papéis (roles) com níveis de acesso
/// </summary>
public static class RoleSeeder
{
    /// <summary>
    /// Definições dos papéis padrão do sistema
    /// </summary>
    public static readonly (string Name, string Description, int Level)[] DefaultRoles =
    {
        ("Admin", "Administrador do sistema com acesso total", 100),
        ("TenantAdmin", "Administrador do tenant", 50),
        ("User", "Usuário comum com acesso básico", 10)
    };

    /// <summary>
    /// Papel padrão para novas contas de usuário
    /// </summary>
    public const string BasicRoleName = "User";

    /// <summary>
    /// Garante que os papéis padrão existam com os níveis de acesso corretos
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        foreach (var (name, description, level) in DefaultRoles)
        {
            var existing = await roleManager.FindByNameAsync(name);

            if (existing == null)
            {
                var result = await roleManager.CreateAsync(new Role
                {
                    Name = name,
                    Description = description,
                    Level = level
                });

                if (result.Succeeded)
                    logger.LogInformation("Papel padrão criado: {RoleName} (nível {Level})", name, level);
                else
                    logger.LogError("Falha ao criar papel padrão {RoleName}: {Errors}",
                        name, string.Join("; ", result.Errors));
            }
            else if (existing.Level != level || existing.Description != description)
            {
                existing.Level = level;
                existing.Description = description;
                await roleManager.UpdateAsync(existing);
                logger.LogInformation("Papel padrão atualizado: {RoleName} (nível {Level})", name, level);
            }
        }
    }
}
