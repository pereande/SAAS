using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para auditoria de requisições
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultiTenancySettings _settings;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="next">Próximo middleware</param>
    /// <param name="settings">Configurações de multi-tenancy</param>
    public AuditMiddleware(
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
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);

            // Logar requisição bem-sucedida
            await LogRequestAsync(context, stopwatch.Elapsed, null);
        }
        catch (Exception ex)
        {
            // Logar requisição com erro
            await LogRequestAsync(context, stopwatch.Elapsed, ex);
            throw;
        }
    }

    /// <summary>
    /// Loga a requisição
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="duration">Duração da requisição</param>
    /// <param name="exception">Exceção (se houver)</param>
    /// <returns>Task</returns>
    private async Task LogRequestAsync(HttpContext context, TimeSpan duration, Exception? exception)
    {
        try
        {
            // Obter o MasterDbContext
            var dbContext = context.RequestServices.GetRequiredService<MasterDbContext>();

            // Criar log de auditoria
            var auditLog = new AuditLog
            {
                TenantId = context.GetTenantId(),
                UserId = context.User?.Identity?.IsAuthenticated == true ? 
                    Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()) : null,
                Action = $"{context.Request.Method}: {context.Request.Path}",
                EntityType = context.GetEndpoint()?.Metadata.GetMetadata<EntityTypeMetadata>()?.Type,
                EntityId = context.Request.RouteValues["id"]?.ToString(),
                ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                RequestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}",
                HttpMethod = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                ErrorMessage = exception?.Message,
                CreatedAt = DateTime.UtcNow
            };

            // Adicionar ao banco de dados
            await dbContext.AuditLogs.AddAsync(auditLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Se falhar ao salvar o log, apenas ignorar
            var logger = context.RequestServices.GetService<ILogger<AuditMiddleware>>();
            logger?.LogError(ex, "Error saving audit log");
        }
    }
}

/// <summary>
/// Metadados para tipo de entidade
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EntityTypeMetadata : Attribute
{
    /// <summary>
    /// Tipo da entidade
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="type">Tipo da entidade</param>
    public EntityTypeMetadata(string type)
    {
        Type = type;
    }
}

/// <summary>
/// Extensions para AuditMiddleware
/// </summary>
public static class AuditMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de auditoria
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditMiddleware>();
    }
}
