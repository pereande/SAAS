using System;
using System.Linq;
using ERP.Shared.Models;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tenant.Tests;

public sealed class TenantDbContextTests
{
    [Fact]
    public void Constructor_rejects_empty_tenant_id()
    {
        var action = () => CreateContext(Guid.Empty, "tenant-a");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_rejects_blank_tenant_name()
    {
        var action = () => CreateContext(Guid.NewGuid(), " ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Global_filter_returns_only_entities_from_current_tenant()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var databaseName = $"tenant-db-{Guid.NewGuid():N}";
        using var contextA = CreateContext(tenantA, "tenant-a", databaseName);
        using var contextB = CreateContext(tenantB, "tenant-b", databaseName);

        contextA.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            CorporateName = "Empresa A"
        });
        contextA.SaveChanges();
        contextB.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            CorporateName = "Empresa B"
        });
        contextB.SaveChanges();

        contextA.Companies.Should().ContainSingle(c => c.CorporateName == "Empresa A");
        contextA.Companies.Should().NotContain(c => c.CorporateName == "Empresa B");
        contextB.Companies.Should().ContainSingle(c => c.CorporateName == "Empresa B");
        contextB.Companies.Should().NotContain(c => c.CorporateName == "Empresa A");
    }

    [Fact]
    public void SaveChanges_assigns_current_tenant_to_new_entity_without_tenant()
    {
        var tenantId = Guid.NewGuid();
        using var context = CreateContext(tenantId, "tenant-a");
        var company = new Company
        {
            Id = Guid.NewGuid(),
            CorporateName = "Empresa A"
        };

        context.Companies.Add(company);
        context.SaveChanges();

        company.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void SaveChanges_rejects_entity_from_another_tenant()
    {
        var currentTenant = Guid.NewGuid();
        using var context = CreateContext(currentTenant, "tenant-a");
        var company = new Company
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            CorporateName = "Empresa indevida"
        };

        context.Companies.Add(company);

        var action = () => context.SaveChanges();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*does not belong to the current tenant*");
    }

    private static TenantDbContext CreateContext(
        Guid tenantId,
        string tenantName,
        string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName ?? $"tenant-db-{Guid.NewGuid():N}")
            .Options;

        return new TenantDbContext(options, tenantId, tenantName);
    }
}
