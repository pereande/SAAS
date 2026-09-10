using ERP.Master.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Master.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Permission
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <summary>
    /// Configura a entidade Permission
    /// </summary>
    /// <param name="builder">EntityTypeBuilder</param>
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        // Chave primária
        builder.HasKey(p => p.Id);

        // Propriedades
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("code");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name");

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("module");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamento: uma permissão pertence a vários papéis (via role_permissions)
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_role_permissions_permission_id");

        // Índices
        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("ix_permissions_code");

        builder.HasIndex(p => p.Module)
            .HasDatabaseName("ix_permissions_module");
    }
}
