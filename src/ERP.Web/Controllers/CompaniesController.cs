using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para empresas (Company) do tenant atual
/// </summary>
public class CompaniesController : TenantCrudController<Company>
{
    public CompaniesController(TenantDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Company> ApplySearch(IQueryable<Company> query, string search)
    {
        return query.Where(c =>
            c.CorporateName.Contains(search) ||
            (c.TradeName != null && c.TradeName.Contains(search)) ||
            c.Cnpj.Contains(search));
    }
}
