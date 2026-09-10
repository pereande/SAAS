using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para clientes (Client) do tenant atual
/// </summary>
public class ClientsController : TenantCrudController<Client>
{
    public ClientsController(TenantDbContext dbContext) : base(dbContext) { }
}
