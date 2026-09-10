using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para notas fiscais (FiscalNote) do tenant atual
/// </summary>
public class FiscalNotesController : TenantCrudController<FiscalNote>
{
    public FiscalNotesController(TenantDbContext dbContext) : base(dbContext) { }
}
