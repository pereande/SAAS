using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para validar se o tenant existe e está ativo
/// </summary>
public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MasterDbContext _masterDbContext;

    public TenantValidationMiddleware(RequestDelegate next, MasterDbContext masterDbContext)
    {
        _next = next;
        _masterDbContext = masterDbContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Obter TenantId do contexto
        if (!context.Items.TryGetValue("TenantId", out var tenantIdObj) || tenantIdObj == null)
        {
            // Se não tem tenant, é admin da plataforma (ou endpoint de autenticação)
            await _next(context);
            return;
        }

        var tenantId = (Guid)tenantIdObj;

        // 2. Validar se tenant existe
        var tenant = await _masterDbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);
            
        if (tenant == null)
            throw new UnauthorizedException($"Tenant com ID {tenantId} não encontrado");

        // 3. Validar se tenant está ativo
        if (tenant.Status != TenantStatus.Active)
            throw new UnauthorizedException($"Tenant está {tenant.Status.ToString().ToLower()}");

        // 4. Validar se assinatura está ativa (se não for trial)
        if (tenant.TrialEnd == null || tenant.TrialEnd < DateTime.UtcNow)
        {
            if (tenant.SubscriptionId == null)
                throw new UnauthorizedException("Assinatura expirada");

            var subscription = await _masterDbContext.Subscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == tenant.SubscriptionId);
                
            if (subscription == null || subscription.Status != SubscriptionStatus.Active)
                throw new UnauthorizedException("Assinatura não está ativa");
        }

        // 5. Validar limite de usuários (opcional - descomentar se necessário)
        // if (tenant.MaxUsers.HasValue)
        // {
        //     var userCount = await _masterDbContext.Users.CountAsync(u => u.TenantId == tenantId);
        //     if (userCount >= tenant.MaxUsers)
        //         throw new UnauthorizedException("Limite de usuários excedido");
        // }

        await _next(context);
    }
}
