using ERP.Shared.Entities;

namespace ERP.Master.Models;

public class Permission : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // Ex: "sales:read", "financial:write"
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty; // Ex: "Sales", "Financial", "Inventory"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Relacionamentos
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
