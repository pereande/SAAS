using System.Security.Claims;
using ERP.Shared.Exceptions;
using ERP.Shared.Settings;
using ERP.Web.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace ERP.Web.Tests;

public sealed class TenantResolutionMiddlewareTests
{
    [Fact]
    public async Task RejectsHeaderThatDoesNotMatchAuthenticatedTenant()
    {
        var expectedTenant = Guid.NewGuid();
        var requestedTenant = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = requestedTenant.ToString();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("tenant_id", expectedTenant.ToString())
        }, "test"));

        var middleware = new TenantResolutionMiddleware(
            _ => Task.CompletedTask,
            Options.Create(new MultiTenancySettings { TenantHeader = "X-Tenant-Id", AllowDefaultTenant = false }));

        var act = () => middleware.InvokeAsync(context);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task AllowsMatchingHeaderAndTokenTenant()
    {
        var tenantId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = tenantId.ToString();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("tenant_id", tenantId.ToString())
        }, "test"));

        var middleware = new TenantResolutionMiddleware(
            _ => Task.CompletedTask,
            Options.Create(new MultiTenancySettings { TenantHeader = "X-Tenant-Id", AllowDefaultTenant = false }));

        await middleware.InvokeAsync(context);

        context.GetTenantId().Should().Be(tenantId);
    }
}
