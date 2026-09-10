using ERP.Shared.Exceptions;
using ERP.Tenant.Infrastructure.Data;
using ERP.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Services;

/// <summary>
/// Factory that resolves the per-tenant <see cref="TenantDbContext"/>
/// using the connection string from the current request's resolved tenant.
/// </summary>
public interface ITenantDbContextFactory
{
    /// <summary>
    /// Creates a <see cref="TenantDbContext"/> for the current tenant.
    /// </summary>
    TenantDbContext Create();
}

/// <summary>
/// Default implementation of <see cref="ITenantDbContextFactory"/>.
/// Reads the <see cref="TenantContext"/> from the current HTTP request
/// (populated by the tenant middleware pipeline) and builds a
/// <see cref="TenantDbContext"/> pointing at the tenant's database.
/// </summary>
public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor
    /// </summary>
    public TenantDbContextFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public TenantDbContext Create()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HTTP context available.");

        var tenantContext = httpContext.GetTenantContext()
            ?? throw new BadRequestException(
                "Tenant not resolved. Provide a tenant identifier via header, subdomain, or token.");

        if (!tenantContext.TenantId.HasValue)
            throw new BadRequestException("Tenant ID is not available.");

        if (string.IsNullOrEmpty(tenantContext.ConnectionString))
            throw new BadRequestException(
                "Tenant connection string is not configured. " +
                "Ensure the tenant has a valid database connection.");

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(tenantContext.ConnectionString)
            .Options;

        return new TenantDbContext(
            options,
            tenantContext.TenantId.Value,
            tenantContext.TenantName ?? string.Empty);
    }
}
