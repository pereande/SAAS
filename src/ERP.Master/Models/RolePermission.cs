using ERP.Shared.Entities;

namespace ERP.Master.Models;

public class RolePermission : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}
