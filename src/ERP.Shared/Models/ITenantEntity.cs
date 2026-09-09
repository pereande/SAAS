using System;

namespace ERP.Shared.Models;

/// <summary>
/// Interface para entidades que pertencem a um tenant
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    Guid TenantId { get; set; }
}
