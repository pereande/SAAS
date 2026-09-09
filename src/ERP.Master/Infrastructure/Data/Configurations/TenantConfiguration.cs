using ERP.Master.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Master.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Tenant
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    /// <summary>
    /// Configura a entidade Tenant
    /// </summary>
    /// <param name="builder">EntityTypeBuilder</param>
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        // Chave primária
        builder.HasKey(t => t.Id);

        // Propriedades
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(t => t.Cnpj)
            .IsRequired()
            .HasMaxLength(18)
            .HasColumnName("cnpj");

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.Property(t => t.Phone)
            .HasMaxLength(20)
            .HasColumnName("phone");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(t => t.DbName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("db_name");

        builder.Property(t => t.ConnectionString)
            .IsRequired()
            .HasColumnName("conn_string");

        builder.Property(t => t.MaxUsers)
            .HasColumnName("max_users");

        builder.Property(t => t.MaxStorage)
            .HasColumnName("max_storage");

        builder.Property(t => t.CurrentStorage)
            .IsRequired()
            .HasColumnName("current_storage");

        builder.Property(t => t.TrialEnd)
            .HasColumnName("trial_end");

        builder.Property(t => t.SubscriptionId)
            .HasColumnName("subscription_id");

        builder.Property(t => t.PlanId)
            .HasColumnName("plan_id");

        builder.Property(t => t.EnabledModules)
            .HasColumnName("modules")
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList());

        builder.Property(t => t.Settings)
            .HasColumnName("settings")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Timestamps
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamentos
        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_tenants_users_tenant_id");

        builder.HasOne(t => t.Subscription)
            .WithOne(s => s.Tenant)
            .HasForeignKey<Tenant>(t => t.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_tenants_subscriptions_subscription_id");

        // Índices
        builder.HasIndex(t => t.Cnpj)
            .IsUnique()
            .HasDatabaseName("ix_tenants_cnpj");

        builder.HasIndex(t => t.Email)
            .HasDatabaseName("ix_tenants_email");

        builder.HasIndex(t => t.DbName)
            .IsUnique()
            .HasDatabaseName("ix_tenants_db_name");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("ix_tenants_status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("ix_tenants_created_at");
    }
}
