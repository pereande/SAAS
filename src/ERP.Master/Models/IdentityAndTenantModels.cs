using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ERP.Shared.Models;

namespace ERP.Master.Models;

public enum UserStatus { Active, Inactive, Blocked, Deleted }
public enum TenantStatus { Trial, Active, Inactive, Suspended, Deleted }
public enum SubscriptionStatus { Trial, Active, PastDue, Suspended, Cancelled, Expired }

public sealed class User : IdentityUser<Guid>
{
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Locale { get; set; } = "pt-BR";
    public string Timezone { get; set; } = "America/Sao_Paulo";
    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public bool EmailVerified { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public string? TwoFactorSecret { get; set; }
    public string? TwoFactorRecoveryCodes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public sealed class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public sealed class UserRole : IdentityUserRole<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

public sealed class UserClaim : IdentityUserClaim<Guid> { }
public sealed class UserLogin : IdentityUserLogin<Guid> { public Guid Id { get; set; } = Guid.NewGuid(); }
public sealed class RoleClaim : IdentityRoleClaim<Guid> { }

public sealed class Permission : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Module { get; set; }
    public string? Action { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public sealed class RolePermission : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public sealed class Tenant : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.Trial;
    public string DbName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public int? MaxUsers { get; set; }
    public long? MaxStorage { get; set; }
    public long CurrentStorage { get; set; }
    public DateTime? TrialEnd { get; set; }
    public Guid? SubscriptionId { get; set; }
    public Guid? PlanId { get; set; }
    public List<string> EnabledModules { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Subscription? Subscription { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}

public sealed class Subscription : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlanId { get; set; }
    public Plan? Plan { get; set; }
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime? TrialStart { get; set; }
    public DateTime? TrialEnd { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public int? MaxUsers { get; set; }
    public int? MaxFilials { get; set; }
    public long? MaxStorage { get; set; }
    public bool IsActive => Status is SubscriptionStatus.Trial or SubscriptionStatus.Active;
}
