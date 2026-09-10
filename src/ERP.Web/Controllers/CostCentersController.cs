using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para centros de custo (CostCenter) do tenant atual
/// </summary>
public class CostCentersController : TenantCrudController<CostCenter>
{
    public CostCentersController(TenantDbContext dbContext) : base(dbContext) { }
}
