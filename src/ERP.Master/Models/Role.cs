using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Representa um papel/perfil de usuário (estende o IdentityRole)
/// </summary>
[Table("roles")]
public class Role : IdentityRole<Guid>, IEntity<Guid>
{
    /// <summary>
    /// Descrição do papel
    /// </summary>
    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// Nível de acesso (quanto maior, mais privilégios)
    /// </summary>
    [Column("level")]
    public int Level { get; set; } = 10;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Usuários com este papel
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Permissões deste papel
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
