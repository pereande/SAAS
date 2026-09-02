using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Shared.Exceptions;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para resolução de tenant
/// Identifica o tenant da requisição e configura o contexto
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultiTenancySettings _settings;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="next">Próximo middleware</param>
    /// <param name="settings">Configurações de multi-tenancy</param>
    public TenantResolutionMiddleware(
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
        try
        {
            // Obter o tenant da requisição
            var tenantContext = ResolveTenant(context);

            if (tenantContext == null && !_settings.AllowDefaultTenant)
            {
                throw new BadRequestException("Tenant not specified. Please provide a tenant identifier.");
            }

            // Adicionar tenant ao contexto HTTP
            context.Items["TenantContext"] = tenantContext;

            // Chamar o próximo middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Se o erro não for tratado, propagar
            throw;
        }
    }

    /// <summary>
    /// Resolve o tenant da requisição
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <returns>TenantContext</returns>
    private TenantContext? ResolveTenant(HttpContext context)
    {
        // 1. Verificar header X-Tenant-Id
        if (context.Request.Headers.TryGetValue(_settings.TenantHeader, out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader, out var tenantId))
            {
                return new TenantContext { TenantId = tenantId, ResolutionMethod = TenantResolutionMethod.Header };
            }
        }

        // 2. Verificar subdomínio (se habilitado)
        if (_settings.UseSubdomain)
        {
            var host = context.Request.Host.Host;
            var subdomain = GetSubdomain(host);
            
            if (!string.IsNullOrEmpty(subdomain))
            {
                // TODO: Buscar tenant pelo subdomínio no banco de dados
                // Por enquanto, retornamos o subdomínio como nome
                return new TenantContext { 
                    TenantName = subdomain, 
                    ResolutionMethod = TenantResolutionMethod.Subdomain 
                };
            }
        }

        // 3. Verificar claim do token JWT (se autenticado)
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
            if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return new TenantContext { TenantId = tenantId, ResolutionMethod = TenantResolutionMethod.Token };
            }
        }

        // 4. Usar tenant padrão (se permitido)
        if (_settings.AllowDefaultTenant && _settings.DefaultTenantId.HasValue)
        {
            return new TenantContext { 
                TenantId = _settings.DefaultTenantId.Value, 
                ResolutionMethod = TenantResolutionMethod.Default 
            };
        }

        return null;
    }

    /// <summary>
    /// Extrai o subdomínio do host
    /// </summary>
    /// <param name="host">Host</param>
    /// <returns>Subdomínio</returns>
    private string? GetSubdomain(string host)
    {
        if (string.IsNullOrEmpty(host))
            return null;

        var parts = host.Split('.');
        
        if (parts.Length <= 2)
            return null;

        // Verificar se o último domínio é o base domain
        var baseParts = _settings.BaseDomain.Split('.');
        var hostBase = string.Join(".", parts.Skip(parts.Length - baseParts.Length));
        
        if (hostBase.Equals(_settings.BaseDomain, StringComparison.OrdinalIgnoreCase))
        {
            return string.Join(".", parts.Take(parts.Length - baseParts.Length));
        }

        return null;
    }
}

/// <summary>
/// Contexto do tenant
/// </summary>
public class TenantContext
{
    /// <summary>
    /// ID do tenant
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Nome do tenant
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// Connection string do banco do tenant
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Método de resolução
    /// </summary>
    public TenantResolutionMethod ResolutionMethod { get; set; } = TenantResolutionMethod.Unknown;

    /// <summary>
    /// Verifica se o tenant foi resolvido
    /// </summary>
    public bool IsResolved => TenantId.HasValue || !string.IsNullOrEmpty(TenantName);
}

/// <summary>
/// Método de resolução do tenant
/// </summary>
public enum TenantResolutionMethod
{
    /// <summary>
    /// Desconhecido
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Header HTTP
    /// </summary>
    Header = 1,

    /// <summary>
    /// Subdomínio
    /// </summary>
    Subdomain = 2,

    /// <summary>
    /// Token JWT
    /// </summary>
    Token = 3,

    /// <summary>
    /// Tenant padrão
    /// </summary>
    Default = 4
}

/// <summary>
/// Extensions para TenantResolutionMiddleware
/// </summary>
public static class TenantResolutionMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de resolução de tenant
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }

    /// <summary>
    /// Obtém o contexto do tenant do HttpContext
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>TenantContext</returns>
    public static TenantContext? GetTenantContext(this HttpContext context)
    {
        return context.Items["TenantContext"] as TenantContext;
    }

    /// <summary>
    /// Obtém o ID do tenant do HttpContext
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>ID do tenant</returns>
    public static Guid? GetTenantId(this HttpContext context)
    {
        var tenantContext = context.GetTenantContext();
        return tenantContext?.TenantId;
    }
}
