using System;
using System.Reflection;
using ERP.Master.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Master.Infrastructure.Data;

/// <summary>
/// DbContext principal para o banco ERP_MASTER
/// Contém todas as tabelas globais da plataforma
/// </summary>
public class MasterDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="options">Opções do DbContext</param>
    public MasterDbContext(DbContextOptions<MasterDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tenants (empresas contratantes)
    /// </summary>
    public DbSet<Tenant> Tenants { get; set; } = null!;

    /// <summary>
    /// Assinaturas
    /// </summary>
    public DbSet<Subscription> Subscriptions { get; set; } = null!;

    /// <summary>
    /// Planos
    /// </summary>
    public DbSet<Plan> Plans { get; set; } = null!;

    /// <summary>
    /// Permissões
    /// </summary>
    public DbSet<Permission> Permissions { get; set; } = null!;

    /// <summary>
    /// Relacionamento perfil-permissão
    /// </summary>
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;

    /// <summary>
    /// Tokens de usuário
    /// </summary>
    public new DbSet<UserToken> UserTokens { get; set; } = null!;

    /// <summary>
    /// Sessões de usuário
    /// </summary>
    public DbSet<UserSession> UserSessions { get; set; } = null!;

    /// <summary>
    /// Tentativas de login
    /// </summary>
    public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;

    /// <summary>
    /// Logs de auditoria
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    /// <summary>
    /// Configuração das entidades
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações de todas as entidades no assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configurações específicas para Identity
        ConfigureIdentity(modelBuilder);

        // Configurações específicas para o schema
        ConfigureSchema(modelBuilder);
    }

    /// <summary>
    /// Configura o schema do banco
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    private static void ConfigureSchema(ModelBuilder modelBuilder)
    {
        // Definir schema padrão
        modelBuilder.HasDefaultSchema("public");
    }

    /// <summary>
    /// Configurações específicas para Identity
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder</param>
    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        // Renomear tabelas do Identity para seguir convenção
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ix_users_email");
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("ix_users_normalized_email");
            b.HasIndex(u => u.UserName).IsUnique().HasDatabaseName("ix_users_username");
            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("ix_users_normalized_username");
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles");
            b.HasIndex(r => r.Name).IsUnique().HasDatabaseName("ix_roles_name");
            b.HasIndex(r => r.NormalizedName).HasDatabaseName("ix_roles_normalized_name");
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("user_roles");
            b.HasKey(ur => ur.Id);
            b.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique().HasDatabaseName("ix_user_roles_user_id_role_id");
        });

        modelBuilder.Entity<UserClaim>(b =>
        {
            b.ToTable("user_claims");
            b.HasIndex(uc => uc.UserId).HasDatabaseName("ix_user_claims_user_id");
        });

        modelBuilder.Entity<UserLogin>(b =>
        {
            b.ToTable("user_logins");
            b.HasKey(l => l.Id);
            b.HasIndex(ul => new { ul.LoginProvider, ul.ProviderKey }).IsUnique().HasDatabaseName("ix_user_logins_login_provider_provider_key");
        });

        modelBuilder.Entity<RoleClaim>(b =>
        {
            b.ToTable("role_claims");
            b.HasIndex(rc => rc.RoleId).HasDatabaseName("ix_role_claims_role_id");
        });

        modelBuilder.Entity<UserToken>(b =>
        {
            b.ToTable("user_tokens");
            b.HasKey(t => t.Id);
            b.HasIndex(ut => new { ut.UserId, ut.LoginProvider, ut.Name }).IsUnique().HasDatabaseName("ix_user_tokens_user_id_login_provider_name");
        });
    }

    /// <summary>
    /// Configurações adicionais
    /// </summary>
    /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configurar para usar snake_case no PostgreSQL
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}
