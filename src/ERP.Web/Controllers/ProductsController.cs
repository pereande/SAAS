using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para produtos (Product) do tenant atual
/// </summary>
public class ProductsController : TenantCrudController<Product>
{
    public ProductsController(TenantDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Product> ApplySearch(IQueryable<Product> query, string search)
    {
        return query.Where(p =>
            p.Name.Contains(search) ||
            p.Code.Contains(search) ||
            (p.Barcode != null && p.Barcode.Contains(search)));
    }
}
