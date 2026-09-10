using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para compras (Purchase) do tenant atual
/// </summary>
public class PurchasesController : TenantCrudController<Purchase>
{
    public PurchasesController(TenantDbContext dbContext) : base(dbContext) { }
}
