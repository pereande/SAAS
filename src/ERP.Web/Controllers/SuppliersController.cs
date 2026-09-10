using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para fornecedores (Supplier) do tenant atual
/// </summary>
public class SuppliersController : TenantCrudController<Supplier>
{
    public SuppliersController(TenantDbContext dbContext) : base(dbContext) { }
}
