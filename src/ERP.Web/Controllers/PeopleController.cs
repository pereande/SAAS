using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para pessoas (Person) do tenant atual
/// </summary>
public class PeopleController : TenantCrudController<Person>
{
    public PeopleController(TenantDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Person> ApplySearch(IQueryable<Person> query, string search)
    {
        return query.Where(p =>
            p.Name.Contains(search) ||
            (p.Document != null && p.Document.Contains(search)));
    }
}
