using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;

namespace ERP.Web.Controllers;

/// <summary>
/// CRUD para condições de pagamento (PaymentTerm) do tenant atual
/// </summary>
public class PaymentTermsController : TenantCrudController<PaymentTerm>
{
    public PaymentTermsController(TenantDbContext dbContext) : base(dbContext) { }
}
