using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para marcas (Brand) do tenant atual
/// </summary>
public class BrandsController : TenantCrudController<Brand>
{
    public BrandsController(TenantDbContext dbContext) : base(dbContext) { }
}
