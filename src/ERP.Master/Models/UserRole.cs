using ERP.Shared.Entities;

namespace ERP.Master.Models;

public class UserRole : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public virtual User? User { get; set; }
    public virtual Role? Role { get; set; }
}
