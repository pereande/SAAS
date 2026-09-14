using System;

namespace ERP.Shared.Models;

/// <summary>
/// Contrato para entidades pertencentes a um tenant.
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
