using System;
using System.Collections.Generic;
using ERP.Master.Models;

namespace ERP.Web.DTOs.Tenants;

/// <summary>
/// Request de listagem de tenants
/// </summary>
public class GetTenantsRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public TenantStatus? Status { get; set; }

    /// <summary>
    /// Número da página
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO de tenant
/// </summary>
public class TenantDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public TenantStatus Status { get; set; }

    /// <summary>
    /// Nome do banco de dados
    /// </summary>
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento
    /// </summary>
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Armazenamento atual
    /// </summary>
    public long CurrentStorage { get; set; }

    /// <summary>
    /// Fim do trial
    /// </summary>
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Assinatura
    /// </summary>
    public SubscriptionDto? Subscription { get; set; }
}

/// <summary>
/// DTO detalhado de tenant
/// </summary>
public class TenantDetailsDto : TenantDto
{
    /// <summary>
    /// Connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Módulos habilitados
    /// </summary>
    public List<string> EnabledModules { get; set; } = new List<string>();

    /// <summary>
    /// Configurações
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Usuários do tenant
    /// </summary>
    public List<TenantUserDto> Users { get; set; } = new List<TenantUserDto>();
}

/// <summary>
/// DTO de usuário do tenant
/// </summary>
public class TenantUserDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Usuário
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// 2FA habilitado
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Último login
    /// </summary>
    public DateTime? LastLogin { get; set; }
}

/// <summary>
/// DTO de assinatura
/// </summary>
public class SubscriptionDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do plano
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Nome do plano
    /// </summary>
    public string? PlanName { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// Data de início
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Data de término
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Início do trial
    /// </summary>
    public DateTime? TrialStart { get; set; }

    /// <summary>
    /// Fim do trial
    /// </summary>
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Método de pagamento
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Status do pagamento
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de filiais
    /// </summary>
    public int? MaxFilials { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento
    /// </summary>
    public long? MaxStorage { get; set; }
}

/// <summary>
/// Request de criação de tenant
/// </summary>
public class CreateTenantRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento
    /// </summary>
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Dias de trial
    /// </summary>
    public int? TrialDays { get; set; }

    /// <summary>
    /// Módulos habilitados
    /// </summary>
    public List<string> EnabledModules { get; set; } = new List<string>();

    /// <summary>
    /// Configurações
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Usuário administrador inicial
    /// </summary>
    public CreateTenantUserRequest? AdminUser { get; set; }

    /// <summary>
    /// Assinatura inicial
    /// </summary>
    public CreateSubscriptionRequest? Subscription { get; set; }
}

/// <summary>
/// Request de criação de usuário do tenant
/// </summary>
public class CreateTenantUserRequest
{
    /// <summary>
    /// Usuário
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Roles
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();
}

/// <summary>
/// Request de criação de assinatura
/// </summary>
public class CreateSubscriptionRequest
{
    /// <summary>
    /// ID do plano
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Data de início
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Dias de trial
    /// </summary>
    public int? TrialDays { get; set; }

    /// <summary>
    /// Módulos
    /// </summary>
    public Dictionary<string, bool> Modules { get; set; } = new Dictionary<string, bool>();
}

/// <summary>
/// Request de atualização de tenant
/// </summary>
public class UpdateTenantRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public TenantStatus? Status { get; set; }

    /// <summary>
    /// Limite máximo de usuários
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Limite máximo de armazenamento
    /// </summary>
    public long? MaxStorage { get; set; }

    /// <summary>
    /// Módulos habilitados
    /// </summary>
    public List<string> EnabledModules { get; set; } = new List<string>();

    /// <summary>
    /// Configurações
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
}
