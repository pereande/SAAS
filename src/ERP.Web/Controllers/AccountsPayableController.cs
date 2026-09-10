using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para contas a pagar (AccountPayable) do tenant atual
/// </summary>
public class AccountsPayableController : TenantCrudController<AccountPayable>
{
    public AccountsPayableController(TenantDbContext dbContext) : base(dbContext) { }
}
