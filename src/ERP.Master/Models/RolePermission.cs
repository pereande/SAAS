using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa o relacionamento entre um papel e uma permissão
/// </summary>
[Table("role_permissions")]
public class RolePermission : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID do papel
    /// </summary>
    [Required]
    [Column("role_id")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// Papel
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// ID da permissão
    /// </summary>
    [Required]
    [Column("permission_id")]
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Permissão
    /// </summary>
    [ForeignKey("PermissionId")]
    public virtual Permission Permission { get; set; } = null!;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
