using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa uma permissão do sistema (ex: "sales.create", "reports.view")
/// </summary>
[Table("permissions")]
public class Permission : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Código único da permissão (ex: "sales.create")
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome da permissão
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da permissão
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Módulo ao qual a permissão pertence (ex: "sales")
    /// </summary>
    [Required]
    [Column("module")]
    [StringLength(50)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Papéis que possuem esta permissão
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
