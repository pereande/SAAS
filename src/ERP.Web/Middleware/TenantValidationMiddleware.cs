using System;
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
/// Middleware para validação de tenant
/// Valida se o tenant existe e está ativo
/// </summary>
public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultiTenancySettings _settings;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="next">Próximo middleware</param>
    /// <param name="settings">Configurações de multi-tenancy</param>
    public TenantValidationMiddleware(
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
        // Obter o contexto do tenant
        var tenantContext = context.GetTenantContext();

        // Se não houver tenant resolvido, pular validação
        if (tenantContext == null || !tenantContext.IsResolved)
        {
            await _next(context);
            return;
        }

        // Se o tenant for resolvido por token (já autenticado), pular validação
        if (tenantContext.ResolutionMethod == TenantResolutionMethod.Token)
        {
            await _next(context);
            return;
        }

        // Validar tenant no banco de dados
        var tenant = await ValidateTenantAsync(context, tenantContext);

        if (tenant == null)
        {
            throw new BadRequestException("Invalid tenant. The specified tenant does not exist or is inactive.");
        }

        // Verificar se o tenant está ativo
        if (tenant.Status != TenantStatus.Active)
        {
            throw new ForbiddenException(
                tenant.Status switch
                {
                    TenantStatus.Inactive => "Tenant is inactive.",
                    TenantStatus.Suspended => "Tenant is suspended.",
                    TenantStatus.Deleted => "Tenant has been deleted.",
                    TenantStatus.Trial => "Tenant trial has expired.",
                    _ => "Tenant is not available."
                });
        }

        // Verificar se a assinatura está ativa
        if (tenant.SubscriptionId.HasValue && tenant.Subscription != null)
        {
            if (!tenant.Subscription.IsActive)
            {
                throw new ForbiddenException("Tenant subscription is not active.");
            }
        }

        // Atualizar o contexto do tenant com as informações do banco
        tenantContext.TenantId = tenant.Id;
        tenantContext.TenantName = tenant.Name;
        tenantContext.ConnectionString = tenant.ConnectionString;

        // Adicionar claims do tenant ao usuário (se autenticado)
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // TODO: Adicionar claims do tenant ao principal
        }

        // Chamar o próximo middleware
        await _next(context);
    }

    /// <summary>
    /// Valida o tenant no banco de dados
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="tenantContext">Contexto do tenant</param>
    /// <returns>Tenant ou null</returns>
    private async Task<Tenant?> ValidateTenantAsync(HttpContext context, TenantContext tenantContext)
    {
        try
        {
            // Obter o MasterDbContext
            var dbContext = context.RequestServices.GetRequiredService<MasterDbContext>();

            Tenant? tenant = null;

            // Buscar por ID
            if (tenantContext.TenantId.HasValue)
            {
                tenant = await dbContext.Tenants
                    .Include(t => t.Subscription)
                    .FirstOrDefaultAsync(t => t.Id == tenantContext.TenantId.Value);
            }
            // Buscar por nome (subdomínio)
            else if (!string.IsNullOrEmpty(tenantContext.TenantName))
            {
                tenant = await dbContext.Tenants
                    .Include(t => t.Subscription)
                    .FirstOrDefaultAsync(t => t.Name == tenantContext.TenantName || 
                                          t.DbName == tenantContext.TenantName);
            }

            return tenant;
        }
        catch (Exception ex)
        {
            // Logar erro e continuar
            var logger = context.RequestServices.GetService<ILogger<TenantValidationMiddleware>>();
            logger?.LogError(ex, "Error validating tenant");
            return null;
        }
    }
}

/// <summary>
/// Extensions para TenantValidationMiddleware
/// </summary>
public static class TenantValidationMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de validação de tenant
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseTenantValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantValidationMiddleware>();
    }
}
