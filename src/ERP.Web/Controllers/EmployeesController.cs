using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para funcionários (Employee) do tenant atual
/// </summary>
public class EmployeesController : TenantCrudController<Employee>
{
    public EmployeesController(TenantDbContext dbContext) : base(dbContext) { }
}
