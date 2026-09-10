using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para contas a receber (AccountReceivable) do tenant atual
/// </summary>
public class AccountsReceivableController : TenantCrudController<AccountReceivable>
{
    public AccountsReceivableController(TenantDbContext dbContext) : base(dbContext) { }
}
