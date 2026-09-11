using ERP.Master.Infrastructure.Data;
using ERP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para resolver o tenant atual com base em:
/// 1. Header: X-Tenant-Id
/// 2. Subdomínio: tenant1.seu-erp.com
/// 3. Token JWT (claim "tenant_id")
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MasterDbContext _masterDbContext;

    public TenantResolutionMiddleware(RequestDelegate next, MasterDbContext masterDbContext)
    {
        _next = next;
        _masterDbContext = masterDbContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Guid? tenantId = null;

        // 1. Tentar resolver via Header X-Tenant-Id
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader, out var headerTenantId))
                tenantId = headerTenantId;
        }

        // 2. Tentar resolver via Subdomínio (ex: tenant1.localhost:5000)
        if (tenantId == null)
        {
            var host = context.Request.Host.Host;
            if (!string.IsNullOrEmpty(host) && host != "localhost" && !host.StartsWith("localhost"))
            {
                // Remover porta se existir e pegar o subdomínio
                var subdomain = host.Split('.')[0];
                
                // Tenta parsear como GUID primeiro (caso usem ID direto no subdomínio)
                if (Guid.TryParse(subdomain, out var subdomainTenantId))
                {
                    tenantId = subdomainTenantId;
                }
                else
                {
                    // Buscar tenant pelo nome do subdomínio (DbName)
                    var tenant = await _masterDbContext.Tenants
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.DbName == subdomain);
                    
                    if (tenant != null)
                        tenantId = tenant.Id;
                }
            }
        }

        // 3. Tentar resolver via Token JWT (se ainda não encontrou)
        if (tenantId == null && context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    // Não valida assinatura aqui apenas para extrair claim, validação real fica no UseAuthentication
                    if (tokenHandler.CanReadToken(token))
                    {
                        var jwtToken = tokenHandler.ReadJwtToken(token);
                        var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenant_id");
                        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tokenTenantId))
                            tenantId = tokenTenantId;
                    }
                }
                catch 
                { 
                    // Token inválido ou mal formado, ignora e deixa o fluxo seguir para falhar no Auth se necessário
                }
            }
        }

        // 4. Se não encontrado, verificar se é endpoint público (Auth)
        if (tenantId == null)
        {
            // Endpoints de autenticação não exigem tenant neste estágio
            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                await _next(context);
                return;
            }

            // Para outros endpoints, exige tenant
            throw new UnauthorizedException("Tenant não especificado. Utilize o header X-Tenant-Id ou um token válido.");
        }

        // 5. Armazenar TenantId e DbName no contexto para uso pelos próximos middlewares
        context.Items["TenantId"] = tenantId;
        
        // Busca rápida o DbName para otimizar o próximo middleware
        var dbName = await _masterDbContext.Tenants
            .AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.DbName)
            .FirstOrDefaultAsync();
            
        context.Items["TenantDbName"] = dbName;

        await _next(context);
    }
}
