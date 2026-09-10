using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Permissão do sistema
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
    /// Código da permissão
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Módulo
    /// </summary>
    [Column("module")]
    [StringLength(50)]
    public string? Module { get; set; }

    /// <summary>
    /// Ação
    /// </summary>
    [Column("action")]
    [StringLength(50)]
    public string? Action { get; set; }

    /// <summary>
    /// Ativa
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Relacionamento perfil-permissão
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
    /// ID do perfil
    /// </summary>
    [Required]
    [Column("role_id")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// Perfil
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role? Role { get; set; }

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
    public virtual Permission? Permission { get; set; }

    /// <summary>
    /// Concedido por
    /// </summary>
    [Column("granted_by")]
    public Guid? GrantedBy { get; set; }

    /// <summary>
    /// Data de concessão
    /// </summary>
    [Required]
    [Column("granted_at")]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}
