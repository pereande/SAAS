using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para filiais (Branch) do tenant atual
/// </summary>
public class BranchesController : TenantCrudController<Branch>
{
    public BranchesController(TenantDbContext dbContext) : base(dbContext) { }
}
