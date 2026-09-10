using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// Consulta de estoque (Inventory) do tenant atual
/// </summary>
public class InventoryController : TenantCrudController<Inventory>
{
    public InventoryController(TenantDbContext dbContext) : base(dbContext) { }
}
