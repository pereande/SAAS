using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Shared.Models;

namespace ERP.Master.Models;

/// <summary>
/// Representa uma tentativa de login
/// </summary>
[Table("login_attempts")]
public class LoginAttempt : IEntity<Guid>
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID do usuário (se conhecido)
    /// </summary>
    [Column("user_id")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// Usuário
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    /// <summary>
    /// E-mail ou usuário tentado
    /// </summary>
    [Required]
    [Column("username")]
    [StringLength(255)]
    public string Username { get; set; } = string.Empty;

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
    /// Login bem-sucedido
    /// </summary>
    [Required]
    [Column("is_success")]
    public bool IsSuccess { get; set; } = false;

    /// <summary>
    /// Falha por senha incorreta
    /// </summary>
    [Column("wrong_password")]
    public bool WrongPassword { get; set; } = false;

    /// <summary>
    /// Falha por usuário não encontrado
    /// </summary>
    [Column("user_not_found")]
    public bool UserNotFound { get; set; } = false;

    /// <summary>
    /// Falha por conta bloqueada
    /// </summary>
    [Column("account_locked")]
    public bool AccountLocked { get; set; } = false;

    /// <summary>
    /// Falha por 2FA
    /// </summary>
    [Column("two_factor_failed")]
    public bool TwoFactorFailed { get; set; } = false;

    /// <summary>
    /// Mensagem de erro
    /// </summary>
    [Column("error_message")]
    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Data da tentativa
    /// </summary>
    [Required]
    [Column("attempt_time")]
    public DateTime AttemptTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID do tenant (se aplicável)
    /// </summary>
    [Column("tenant_id")]
    public Guid? TenantId { get; set; }
}
