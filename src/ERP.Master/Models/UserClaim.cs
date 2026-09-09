using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Claim de um usuário (estende o IdentityUserClaim)
/// </summary>
public class UserClaim : IdentityUserClaim<Guid>
{
}
