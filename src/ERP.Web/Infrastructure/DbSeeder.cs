using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Settings;
using ERP.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ERP.Web.Infrastructure;

/// <summary>
/// Inicializa o banco master com os dados essenciais:
/// perfis, permissões, planos, usuário admin da plataforma e tenant de demonstração.
/// Executado no startup, é idempotente (não duplica dados).
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Senha padrão dos usuários de demonstração
    /// </summary>
    public const string DefaultPassword = "Admin@123";

    private static readonly (string Code, string Name, string Module, string Action)[] PermissionSeed =
    {
        ("tenants:read", "View Tenants", "system", "read"),
        ("tenants:create", "Create Tenants", "system", "create"),
        ("tenants:update", "Update Tenants", "system", "update"),
        ("tenants:delete", "Delete Tenants", "system", "delete"),
        ("tenants:manage", "Manage Tenants", "system", "manage"),

        ("users:read", "View Users", "system", "read"),
        ("users:create", "Create Users", "system", "create"),
        ("users:update", "Update Users", "system", "update"),
        ("users:delete", "Delete Users", "system", "delete"),
        ("users:manage", "Manage Users", "system", "manage"),

        ("roles:read", "View Roles", "system", "read"),
        ("roles:create", "Create Roles", "system", "create"),
        ("roles:update", "Update Roles", "system", "update"),
        ("roles:delete", "Delete Roles", "system", "delete"),
        ("roles:manage", "Manage Roles", "system", "manage"),

        ("products:read", "View Products", "inventory", "read"),
        ("products:create", "Create Products", "inventory", "create"),
        ("products:update", "Update Products", "inventory", "update"),
        ("products:delete", "Delete Products", "inventory", "delete"),
        ("products:manage", "Manage Products", "inventory", "manage"),

        ("inventory:read", "View Inventory", "inventory", "read"),
        ("inventory:update", "Update Inventory", "inventory", "update"),
        ("inventory:manage", "Manage Inventory", "inventory", "manage"),

        ("sales:read", "View Sales", "sales", "read"),
        ("sales:create", "Create Sales", "sales", "create"),
        ("sales:update", "Update Sales", "sales", "update"),
        ("sales:delete", "Delete Sales", "sales", "delete"),
        ("sales:manage", "Manage Sales", "sales", "manage"),
        ("sales:cancel", "Cancel Sales", "sales", "cancel"),
        ("sales:confirm", "Confirm Sales", "sales", "confirm"),

        ("purchases:read", "View Purchases", "purchases", "read"),
        ("purchases:create", "Create Purchases", "purchases", "create"),
        ("purchases:update", "Update Purchases", "purchases", "update"),
        ("purchases:delete", "Delete Purchases", "purchases", "delete"),
        ("purchases:manage", "Manage Purchases", "purchases", "manage"),
        ("purchases:cancel", "Cancel Purchases", "purchases", "cancel"),
        ("purchases:confirm", "Confirm Purchases", "purchases", "confirm"),
        ("purchases:receive", "Receive Purchases", "purchases", "receive"),

        ("accounts_payable:read", "View Accounts Payable", "financial", "read"),
        ("accounts_payable:create", "Create Accounts Payable", "financial", "create"),
        ("accounts_payable:update", "Update Accounts Payable", "financial", "update"),
        ("accounts_payable:delete", "Delete Accounts Payable", "financial", "delete"),
        ("accounts_payable:pay", "Pay Accounts Payable", "financial", "pay"),

        ("accounts_receivable:read", "View Accounts Receivable", "financial", "read"),
        ("accounts_receivable:create", "Create Accounts Receivable", "financial", "create"),
        ("accounts_receivable:update", "Update Accounts Receivable", "financial", "update"),
        ("accounts_receivable:delete", "Delete Accounts Receivable", "financial", "delete"),
        ("accounts_receivable:receive", "Receive Accounts Receivable", "financial", "receive"),

        ("financial_entries:read", "View Financial Entries", "financial", "read"),
        ("financial_entries:create", "Create Financial Entries", "financial", "create"),
        ("financial_entries:update", "Update Financial Entries", "financial", "update"),
        ("financial_entries:delete", "Delete Financial Entries", "financial", "delete"),
        ("financial_entries:reconcile", "Reconcile Financial Entries", "financial", "reconcile"),

        ("fiscal_notes:read", "View Fiscal Notes", "fiscal", "read"),
        ("fiscal_notes:create", "Create Fiscal Notes", "fiscal", "create"),
        ("fiscal_notes:update", "Update Fiscal Notes", "fiscal", "update"),
        ("fiscal_notes:delete", "Delete Fiscal Notes", "fiscal", "delete"),
        ("fiscal_notes:authorize", "Authorize Fiscal Notes", "fiscal", "authorize"),
        ("fiscal_notes:cancel", "Cancel Fiscal Notes", "fiscal", "cancel"),

        ("fiscal_configurations:read", "View Fiscal Configurations", "fiscal", "read"),
        ("fiscal_configurations:update", "Update Fiscal Configurations", "fiscal", "update"),

        ("reports:read", "View Reports", "reports", "read"),
        ("reports:custom", "Custom Reports", "reports", "custom"),

        ("dashboard:read", "View Dashboard", "dashboard", "read")
    };

    /// <summary>
    /// Executa todo o seed do banco master
    /// </summary>
    /// <param name="services">Service provider do escopo atual</param>
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<MasterDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var dbSettings = services.GetRequiredService<IOptions<DatabaseSettings>>().Value;
        var initializer = services.GetRequiredService<TenantDatabaseInitializer>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await SeedRolesAsync(roleManager);
        await SeedPermissionsAsync(db);
        await SeedAdminRolePermissionsAsync(db, roleManager);
        await SeedPlansAsync(db);
        await SeedSuperAdminAsync(db, userManager, logger);
        await SeedDemoTenantAsync(db, userManager, dbSettings, initializer, logger);

        logger.LogInformation("Database seeding completed");
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        var roles = new[]
        {
            ("Admin", "Administrador do sistema"),
            ("TenantAdmin", "Administrador do tenant"),
            ("User", "Usuário comum")
        };

        foreach (var (name, description) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                await roleManager.CreateAsync(new Role { Name = name, Description = description });
            }
        }
    }

    private static async Task SeedPermissionsAsync(MasterDbContext db)
    {
        var existingCodes = await db.Permissions.Select(p => p.Code).ToListAsync();
        var newPermissions = PermissionSeed
            .Where(p => !existingCodes.Contains(p.Code))
            .Select(p => new Permission
            {
                Code = p.Code,
                Name = p.Name,
                Description = p.Name,
                Module = p.Module,
                Action = p.Action
            })
            .ToList();

        if (newPermissions.Count > 0)
        {
            await db.Permissions.AddRangeAsync(newPermissions);
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedAdminRolePermissionsAsync(MasterDbContext db, RoleManager<Role> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole == null)
        {
            return;
        }

        var existingPermissionIds = await db.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var allPermissionIds = await db.Permissions.Select(p => p.Id).ToListAsync();

        var newRolePermissions = allPermissionIds
            .Where(id => !existingPermissionIds.Contains(id))
            .Select(id => new RolePermission { RoleId = adminRole.Id, PermissionId = id })
            .ToList();

        if (newRolePermissions.Count > 0)
        {
            await db.RolePermissions.AddRangeAsync(newRolePermissions);
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedPlansAsync(MasterDbContext db)
    {
        if (await db.Plans.AnyAsync())
        {
            return;
        }

        await db.Plans.AddRangeAsync(
            new Plan
            {
                Name = "Basic", Code = "basic",
                Description = "Plano básico para pequenas empresas",
                MonthlyPrice = 99.00m, YearlyPrice = 990.00m, TrialDays = 30,
                MaxUsers = 5, MaxFilials = 1, MaxStorage = 1073741824,
                IncludedModules = new List<string> { ERPModules.Sales, ERPModules.Purchases, ERPModules.Inventory, ERPModules.Financial },
                IncludedFeatures = new List<string> { "basic_reports", "email_support" },
                IsDefault = true, DisplayOrder = 1
            },
            new Plan
            {
                Name = "Professional", Code = "pro",
                Description = "Plano profissional para empresas em crescimento",
                MonthlyPrice = 299.00m, YearlyPrice = 2990.00m, TrialDays = 30,
                MaxUsers = 20, MaxFilials = 5, MaxStorage = 10737418240,
                IncludedModules = new List<string> { ERPModules.Sales, ERPModules.Purchases, ERPModules.Inventory, ERPModules.Financial, ERPModules.Fiscal, ERPModules.CRM },
                IncludedFeatures = new List<string> { "advanced_reports", "priority_support", "api_access" },
                DisplayOrder = 2
            },
            new Plan
            {
                Name = "Enterprise", Code = "enterprise",
                Description = "Plano enterprise para grandes empresas",
                MonthlyPrice = 999.00m, YearlyPrice = 9990.00m, TrialDays = 30,
                MaxUsers = 100, MaxFilials = 50, MaxStorage = 107374182400,
                IncludedModules = new List<string>(ERPModules.All),
                IncludedFeatures = new List<string> { "custom_reports", "24_7_support", "dedicated_account_manager", "custom_integrations" },
                DisplayOrder = 3
            });

        await db.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(MasterDbContext db, UserManager<User> userManager, ILogger logger)
    {
        const string adminEmail = "admin@erpsaas.com";

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user != null)
        {
            await AddUserToRoleAsync(db, user, "Admin");
            return;
        }

        user = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin",
            EmailVerified = true,
            Status = UserStatus.Active
        };

        var result = await userManager.CreateAsync(user, DefaultPassword);
        if (result.Succeeded)
        {
            logger.LogInformation("Super admin user created: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create super admin user: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await AddUserToRoleAsync(db, user, "Admin");
    }

    /// <summary>
    /// Vincula um usuário a um perfil.
    /// Nota: UserManager.AddToRoleAsync não pode ser usado porque UserRole tem PK única
    /// (Id) e o UserStore do Identity espera chave composta (UserId + RoleId).
    /// </summary>
    private static async Task AddUserToRoleAsync(MasterDbContext db, User user, string roleName)
    {
        var normalizedRole = roleName.ToUpperInvariant();
        var role = await db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRole);
        if (role == null)
        {
            return;
        }

        var exists = await db.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
        if (!exists)
        {
            await db.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedDemoTenantAsync(
        MasterDbContext db,
        UserManager<User> userManager,
        DatabaseSettings dbSettings,
        TenantDatabaseInitializer initializer,
        ILogger logger)
    {
        if (await db.Tenants.AnyAsync())
        {
            return;
        }

        const string dbName = "erp_tenant_demo";
        var connectionBuilder = new Npgsql.NpgsqlConnectionStringBuilder(dbSettings.MasterConnectionString)
        {
            Database = dbName
        };

        var tenant = new ERP.Master.Models.Tenant
        {
            Name = "Empresa Demo LTDA",
            Cnpj = "12345678000199",
            Email = "demo@erpsaas.com.br",
            Phone = "(11) 1234-5678",
            Status = TenantStatus.Active,
            DbName = dbName,
            ConnectionString = connectionBuilder.ConnectionString,
            MaxUsers = 10,
            TrialEnd = DateTime.UtcNow.AddDays(30),
            EnabledModules = new List<string>
            {
                ERPModules.Sales, ERPModules.Purchases, ERPModules.Inventory,
                ERPModules.Financial, ERPModules.Fiscal, ERPModules.Reports, ERPModules.Dashboard
            }
        };

        await db.Tenants.AddAsync(tenant);
        await db.SaveChangesAsync();

        await initializer.EnsureTenantDatabaseAsync(tenant);

        const string demoAdminEmail = "admin@demo.com.br";
        if (await userManager.FindByEmailAsync(demoAdminEmail) == null)
        {
            var user = new User
            {
                UserName = demoAdminEmail,
                Email = demoAdminEmail,
                FirstName = "Admin",
                LastName = "Demo",
                EmailVerified = true,
                Status = UserStatus.Active,
                TenantId = tenant.Id
            };

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await AddUserToRoleAsync(db, user, "TenantAdmin");
                logger.LogInformation("Demo tenant admin created: {Email}", demoAdminEmail);
            }
            else
            {
                logger.LogError("Failed to create demo tenant admin: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        logger.LogInformation("Demo tenant created: {TenantId} ({Name})", tenant.Id, tenant.Name);
    }
}
