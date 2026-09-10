using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para categorias de produtos (ProductCategory) do tenant atual
/// </summary>
public class ProductCategoriesController : TenantCrudController<ProductCategory>
{
    public ProductCategoriesController(TenantDbContext dbContext) : base(dbContext) { }
}
