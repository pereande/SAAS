using Microsoft.EntityFrameworkCore;
using ERP.Master.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ERP.Master.Data;

public class MasterDbContext : IdentityDbContext<User, Role, Guid, 
    IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, 
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<LoginAttempt> LoginAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.DbName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Cnpj).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configurar Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Tenant)
                  .WithOne(e => e.Subscription)
                  .HasForeignKey<Subscription>(e => e.TenantId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configurar Permission
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Module).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configurar RolePermission
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Role)
                  .WithMany(e => e.RolePermissions)
                  .HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.Permission)
                  .WithMany(e => e.RolePermissions)
                  .HasForeignKey(e => e.PermissionId);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
        });

        // Configurar UserRole (Identity)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Role)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.RoleId);
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        // Configurar UserSession
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RefreshToken).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(e => e.UserSessions)
                  .HasForeignKey(e => e.UserId);
            entity.HasIndex(e => e.RefreshToken).IsUnique();
        });

        // Configurar AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.Entity).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
        });

        // Configurar LoginAttempt
        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Email);
        });

        // Configurar User (Identity)
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail);
            entity.HasIndex(e => e.NormalizedUserName);
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.HasOne(e => e.Tenant)
                  .WithMany(e => e.Users)
                  .HasForeignKey(e => e.TenantId);
        });

        // Configurar Role (Identity)
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
