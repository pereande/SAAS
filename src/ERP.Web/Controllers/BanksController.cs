using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para bancos (Bank) do tenant atual
/// </summary>
public class BanksController : TenantCrudController<Bank>
{
    public BanksController(TenantDbContext dbContext) : base(dbContext) { }
}
