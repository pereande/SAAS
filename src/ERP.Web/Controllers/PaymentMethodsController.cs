using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para formas de pagamento (PaymentMethod) do tenant atual
/// </summary>
public class PaymentMethodsController : TenantCrudController<PaymentMethod>
{
    public PaymentMethodsController(TenantDbContext dbContext) : base(dbContext) { }
}
