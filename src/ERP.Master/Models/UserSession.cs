using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa uma sessão de usuário
/// </summary>
[Table("user_sessions")]
public class UserSession : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID do usuário
    /// </summary>
    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Usuário
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// ID da sessão
    /// </summary>
    [Required]
    [Column("session_id")]
    [StringLength(100)]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Token de acesso JWT
    /// </summary>
    [Column("access_token")]
    [StringLength(1000)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Token de refresh
    /// </summary>
    [Column("refresh_token")]
    [StringLength(1000)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// IP do cliente
    /// </summary>
    [Column("client_ip")]
    [StringLength(45)]
    public string? ClientIp { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    [Column("user_agent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data de expiração
    /// </summary>
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Sessão ativa
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

    /// <summary>
    /// Data de atualização
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// ID do tenant (se aplicável)
    /// </summary>
    [Column("tenant_id")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Verifica se a sessão está expirada
    /// </summary>
    [NotMapped]
    public bool IsExpired => ExpiresAt != null && ExpiresAt <= DateTime.UtcNow;
}
