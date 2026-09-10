using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Shared.Models;

/// <summary>
/// Entidade base com ID e timestamps
/// </summary>
/// <typeparam name="TKey">Tipo do ID</typeparam>
public abstract class BaseEntity<TKey> : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// ID único da entidade
    /// </summary>
    [Key]
    [Column("id")]
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Data de criação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID do usuário que criou
    /// </summary>
    [Column("created_by")]
    [StringLength(450)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// ID do usuário que atualizou
    /// </summary>
    [Column("updated_by")]
    [StringLength(450)]
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Interface genérica para entidades
/// </summary>
/// <typeparam name="TKey">Tipo do ID</typeparam>
public interface IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// ID único da entidade
    /// </summary>
    TKey Id { get; set; }
}

/// <summary>
/// Interface para entidades que pertencem a um tenant
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    Guid TenantId { get; set; }
}
