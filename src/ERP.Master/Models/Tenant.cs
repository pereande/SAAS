using ERP.Shared.Entities;

namespace ERP.Master.Models;

public class Tenant : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string? ConnectionString { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Trial;
    public Guid? PlanId { get; set; }
    public Guid? SubscriptionId { get; set; }
    public int MaxUsers { get; set; } = 5;
    public long MaxStorage { get; set; } = 1073741824; // 1GB em bytes
    public DateTime? TrialEnd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Módulos habilitados para este tenant
    public List<string> EnabledModules { get; set; } = new();
    
    // Relacionamentos
    public virtual Subscription? Subscription { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
