using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para lançamentos financeiros (FinancialEntry) do tenant atual
/// </summary>
public class FinancialEntriesController : TenantCrudController<FinancialEntry>
{
    public FinancialEntriesController(TenantDbContext dbContext) : base(dbContext) { }
}
