using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para vendas (Sale) do tenant atual
/// </summary>
public class SalesController : TenantCrudController<Sale>
{
    public SalesController(TenantDbContext dbContext) : base(dbContext) { }
}
