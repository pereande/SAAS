using System;
using System.Collections.Generic;
using ERP.Master.Models;

namespace ERP.Web.DTOs.Users;

/// <summary>
/// Request de listagem de usuários
/// </summary>
public class GetUsersRequest
{
    /// <summary>
    /// Busca por nome, usuário ou e-mail
    /// </summary>
    public string Search { get; set; } = string.Empty;

    /// <summary>
    /// Filtrar por nome do papel
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Filtrar por status do usuário
    /// </summary>
    public UserStatus? Status { get; set; }

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
/// DTO de papel com nível de acesso
/// </summary>
public class RoleDto
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
    /// Descrição
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nível de acesso
    /// </summary>
    public int Level { get; set; }
}

/// <summary>
/// Request de criação de usuário
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha inicial
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Tenant do usuário (opcional — null para usuário global)
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Papéis a atribuir (opcional — sem informar, recebe o papel básico "User")
    /// </summary>
    public List<Guid>? RoleIds { get; set; }
}

/// <summary>
/// Request de atualização dos papéis de um usuário
/// </summary>
public class UpdateUserRolesRequest
{
    /// <summary>
    /// IDs dos papéis que o usuário terá após a atualização
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}

/// <summary>
/// DTO de usuário para o painel administrativo
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// E-mail verificado
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Status do usuário
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Papéis atribuídos
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// IDs dos papéis atribuídos
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new List<Guid>();

    /// <summary>
    /// Último login
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Tenant e assinatura do usuário (null para usuários globais)
    /// </summary>
    public UserTenantDto? Tenant { get; set; }
}

/// <summary>
/// DTO resumido do tenant com status da assinatura
/// </summary>
public class UserTenantDto
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do tenant
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Status do tenant
    /// </summary>
    public TenantStatus Status { get; set; }

    /// <summary>
    /// Nome do plano da assinatura (null se não houver assinatura)
    /// </summary>
    public string? PlanName { get; set; }

    /// <summary>
    /// Status da assinatura (null se não houver assinatura)
    /// </summary>
    public SubscriptionStatus? SubscriptionStatus { get; set; }

    /// <summary>
    /// Data de término da assinatura
    /// </summary>
    public DateTime? SubscriptionEndDate { get; set; }
}
