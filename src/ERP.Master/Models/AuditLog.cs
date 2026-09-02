using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa um log de auditoria do sistema
/// </summary>
[Table("audit_logs")]
public class AuditLog : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID do tenant (NULL para logs da plataforma)
    /// </summary>
    [Column("tenant_id")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// ID do usuário
    /// </summary>
    [Column("user_id")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// Usuário
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    /// <summary>
    /// Ação executada
    /// </summary>
    [Required]
    [Column("action")]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Entidade afetada
    /// </summary>
    [Column("entity_type")]
    [StringLength(100)]
    public string? EntityType { get; set; }

    /// <summary>
    /// ID da entidade afetada
    /// </summary>
    [Column("entity_id")]
    [StringLength(100)]
    public string? EntityId { get; set; }

    /// <summary>
    /// Dados antigos (JSON)
    /// </summary>
    [Column("old_values")]
    public string? OldValues { get; set; }

    /// <summary>
    /// Novos dados (JSON)
    /// </summary>
    [Column("new_values")]
    public string? NewValues { get; set; }

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
    /// URL da requisição
    /// </summary>
    [Column("request_url")]
    [StringLength(1000)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Método HTTP
    /// </summary>
    [Column("http_method")]
    [StringLength(10)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Status da resposta
    /// </summary>
    [Column("status_code")]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Mensagem de erro
    /// </summary>
    [Column("error_message")]
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Data da ação
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
