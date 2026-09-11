using ERP.Master.Infrastructure.Data;
using ERP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para configurar o TenantDbContext com a connection string do tenant
/// </summary>
public class TenantDbContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MasterDbContext _masterDbContext;

    public TenantDbContextMiddleware(RequestDelegate next, MasterDbContext masterDbContext)
    {
        _next = next;
        _masterDbContext = masterDbContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Obter TenantId e DbName do contexto
        if (!context.Items.TryGetValue("TenantId", out var tenantIdObj) || tenantIdObj == null)
        {
            // Se não tem tenant, usar conexão padrão (ou pular)
            // Isso é válido para endpoints da plataforma (admin da plataforma)
            await _next(context);
            return;
        }

        var tenantId = (Guid)tenantIdObj;
        var dbName = context.Items["TenantDbName"] as string;

        if (string.IsNullOrEmpty(dbName))
        {
            // Obter DbName do tenant se ainda não estiver no contexto
            dbName = await _masterDbContext.Tenants
                .AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.DbName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(dbName))
                throw new UnauthorizedException("Tenant não tem banco de dados configurado");
        }

        // 2. Obter connection string completa do tenant
        var tenant = await _masterDbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);
            
        if (tenant == null || string.IsNullOrEmpty(tenant.ConnectionString))
            throw new UnauthorizedException("Connection string do tenant não encontrada");

        // 3. Armazenar ConnectionString no contexto para uso pelo TenantDbContextFactory ou injeção
        context.Items["TenantConnectionString"] = tenant.ConnectionString;
        context.Items["TenantId"] = tenantId;

        await _next(context);
    }
}
