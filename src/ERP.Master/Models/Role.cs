using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Perfil (role) do sistema
/// </summary>
[Table("roles")]
public class Role : IdentityRole<Guid>
{
    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Permissões do perfil
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Claim de usuário (Identity)
/// </summary>
public class UserClaim : IdentityUserClaim<Guid>
{
}

/// <summary>
/// Relacionamento usuário-perfil (Identity)
/// </summary>
[Table("user_roles")]
public class UserRole : IdentityUserRole<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Perfil
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role? Role { get; set; }
}

/// <summary>
/// Login externo (Identity)
/// </summary>
[Table("user_logins")]
public class UserLogin : IdentityUserLogin<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Claim de perfil (Identity)
/// </summary>
public class RoleClaim : IdentityRoleClaim<Guid>
{
}
