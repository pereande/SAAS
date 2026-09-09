using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Claim de um papel (estende o IdentityRoleClaim)
/// </summary>
public class RoleClaim : IdentityRoleClaim<Guid>
{
}
