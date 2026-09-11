using ERP.Shared.Entities;

namespace ERP.Master.Models;

public class LoginAttempt : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSuccess { get; set; }
    public bool WrongPassword { get; set; }
    public bool TwoFactorFailed { get; set; }
    public bool AccountLocked { get; set; }
    public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public virtual User? User { get; set; }
}
