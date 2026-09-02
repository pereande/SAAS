using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Exceptions;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para autorização baseada em RBAC
/// </summary>
public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultiTenancySettings _settings;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="next">Próximo middleware</param>
    /// <param name="settings">Configurações de multi-tenancy</param>
    public AuthorizationMiddleware(
        RequestDelegate next,
        IOptions<MultiTenancySettings> settings)
    {
        _next = next;
        _settings = settings.Value;
    }

    /// <summary>
    /// Invoca o middleware
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Obter o endpoint
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // Verificar se o endpoint tem metadados de autorização
        var authMetadata = endpoint.Metadata.GetMetadata<AuthorizationMetadata>();

        if (authMetadata == null)
        {
            await _next(context);
            return;
        }

        // Verificar se o usuário está autenticado
        if (!context.User?.Identity?.IsAuthenticated == true)
        {
            throw new UnauthorizedException("Authentication required.");
        }

        // Verificar permissões
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenantId = context.GetTenantId();

        if (!string.IsNullOrEmpty(userId) && tenantId.HasValue)
        {
            // Validar permissão do usuário
            var isAuthorized = await ValidateUserPermissionAsync(context, userId, tenantId.Value, authMetadata);

            if (!isAuthorized)
            {
                throw new ForbiddenException("You do not have permission to perform this action.");
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Valida a permissão do usuário
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="userId">ID do usuário</param>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="metadata">Metadados de autorização</param>
    /// <returns>True se autorizado</returns>
    private async Task<bool> ValidateUserPermissionAsync(
        HttpContext context,
        string userId,
        Guid tenantId,
        AuthorizationMetadata metadata)
    {
        try
        {
            // Obter o MasterDbContext
            var dbContext = context.RequestServices.GetRequiredService<MasterDbContext>();

            // Obter o usuário com roles e permissões
            var user = await dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId) && u.TenantId == tenantId);

            if (user == null)
            {
                return false;
            }

            // Verificar se o usuário está ativo
            if (user.Status != UserStatus.Active)
            {
                return false;
            }

            // Verificar roles
            if (metadata.RequiredRoles != null && metadata.RequiredRoles.Any())
            {
                var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
                var hasRequiredRole = metadata.RequiredRoles.Any(r => userRoles.Contains(r));

                if (!hasRequiredRole)
                {
                    return false;
                }
            }

            // Verificar permissões
            if (metadata.RequiredPermissions != null && metadata.RequiredPermissions.Any())
            {
                var userPermissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Code)
                    .ToList();

                var hasRequiredPermission = metadata.RequiredPermissions
                    .Any(p => userPermissions.Contains(p));

                if (!hasRequiredPermission)
                {
                    return false;
                }
            }

            // Verificar módulos
            if (metadata.RequiredModules != null && metadata.RequiredModules.Any())
            {
                var tenant = await dbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == tenantId);

                if (tenant == null)
                {
                    return false;
                }

                var hasRequiredModule = metadata.RequiredModules
                    .Any(m => tenant.EnabledModules.Contains(m));

                if (!hasRequiredModule)
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetService<ILogger<AuthorizationMiddleware>>();
            logger?.LogError(ex, "Error validating user permission");
            return false;
        }
    }
}

/// <summary>
/// Metadados de autorização
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Roles necessárias
    /// </summary>
    public string[]? RequiredRoles { get; set; }

    /// <summary>
    /// Permissões necessárias
    /// </summary>
    public string[]? RequiredPermissions { get; set; }

    /// <summary>
    /// Módulos necessários
    /// </summary>
    public string[]? RequiredModules { get; set; }
}

/// <summary>
/// Metadados de autorização
/// </summary>
public class AuthorizationMetadata
{
    /// <summary>
    /// Roles necessárias
    /// </summary>
    public string[]? RequiredRoles { get; set; }

    /// <summary>
    /// Permissões necessárias
    /// </summary>
    public string[]? RequiredPermissions { get; set; }

    /// <summary>
    /// Módulos necessários
    /// </summary>
    public string[]? RequiredModules { get; set; }
}

/// <summary>
/// Extensions para AuthorizationMiddleware
/// </summary>
public static class AuthorizationMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de autorização
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthorizationMiddleware>();
    }
}
