using ERP.Master.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Master.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade RolePermission (relacionamento Papel ↔ Permissão)
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    /// <summary>
    /// Configura a entidade RolePermission
    /// </summary>
    /// <param name="builder">EntityTypeBuilder</param>
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        // Chave primária
        builder.HasKey(rp => rp.Id);

        // Propriedades
        builder.Property(rp => rp.RoleId)
            .IsRequired()
            .HasColumnName("role_id");

        builder.Property(rp => rp.PermissionId)
            .IsRequired()
            .HasColumnName("permission_id");

        builder.Property(rp => rp.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamento: PapelPermissão → Papel (N:1)
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_role_permissions_role_id");

        // Relacionamento: PapelPermissão → Permissão (N:1)
        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_role_permissions_permission_id");

        // Índices
        // Evita que o mesmo papel receba a mesma permissão duas vezes
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("ix_role_permissions_role_id_permission_id");

        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("ix_role_permissions_permission_id");
    }
}
