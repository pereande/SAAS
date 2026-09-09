using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP.Master.Models;

/// <summary>
/// Relacionamento entre usuário e papel (estende o IdentityUserRole com ID próprio)
/// </summary>
[Table("user_roles")]
public class UserRole : IdentityUserRole<Guid>, IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Usuário
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Papel
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}
