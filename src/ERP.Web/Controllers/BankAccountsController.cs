using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para contas bancárias (BankAccount) do tenant atual
/// </summary>
public class BankAccountsController : TenantCrudController<BankAccount>
{
    public BankAccountsController(TenantDbContext dbContext) : base(dbContext) { }
}
