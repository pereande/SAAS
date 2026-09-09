using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Constants;
using ERP.Web.DTOs.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de tenants
/// </summary>
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TenantsController : BaseController
{
    private readonly MasterDbContext _dbContext;
    private readonly ILogger<TenantsController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public TenantsController(MasterDbContext dbContext, ILogger<TenantsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os tenants
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantsRequest request)
    {
        try
        {
            var query = _dbContext.Tenants
                .Include(t => t.Subscription)
                .ThenInclude(s => s.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(t => t.Name.Contains(request.Name));

            if (!string.IsNullOrWhiteSpace(request.Cnpj))
                query = query.Where(t => t.Cnpj == request.Cnpj);

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Where(t => t.Email.Contains(request.Email));

            if (request.Status.HasValue)
                query = query.Where(t => t.Status == request.Status);

            query = query.OrderBy(t => t.Name);

            var totalCount = await query.CountAsync();
            var tenants = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var response = new PagedResponse<TenantDto>
            {
                Data = tenants.Select(MapToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenants");
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Obtém um tenant pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.Subscription).ThenInclude(s => s.Plan)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
                return NotFound(ErrorMessages.TenantNotFound);

            return Success(MapToDetailsDto(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant {TenantId}", id);
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Cria um novo tenant
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            if (await _dbContext.Tenants.AnyAsync(t => t.Cnpj == request.Cnpj))
                return ValidationError(new Dictionary<string, List<string>>
                    { { "cnpj", new List<string> { "CNPJ already exists" } } });

            if (await _dbContext.Tenants.AnyAsync(t => t.Email == request.Email))
                return ValidationError(new Dictionary<string, List<string>>
                    { { "email", new List<string> { "Email already exists" } } });

            var dbName = $"erp_tenant_{Guid.NewGuid():N}".ToLower();
            var connectionString = GetTenantConnectionString(dbName);

            var tenant = new ERP.Master.Models.Tenant
            {
                Name = request.Name,
                Cnpj = request.Cnpj,
                Email = request.Email,
                Phone = request.Phone,
                Status = request.Status,
                DbName = dbName,
                ConnectionString = connectionString,
                MaxUsers = request.MaxUsers,
                MaxStorage = request.MaxStorage,
                CurrentStorage = 0,
                TrialEnd = request.TrialDays.HasValue ? DateTime.UtcNow.AddDays(request.TrialDays.Value) : null,
                EnabledModules = request.EnabledModules ?? new List<string>(),
                Settings = request.Settings ?? new Dictionary<string, string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();

            if (request.AdminUser != null)
                await CreateTenantAdminUserAsync(tenant, request.AdminUser);

            if (request.Subscription != null)
                await CreateTenantSubscriptionAsync(tenant, request.Subscription);

            _logger.LogInformation("Tenant created: {TenantId} - {Name}", tenant.Id, tenant.Name);

            return Success(MapToDto(tenant), "Tenant created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Atualiza um tenant
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound(ErrorMessages.TenantNotFound);

            tenant.Name = request.Name ?? tenant.Name;
            tenant.Phone = request.Phone ?? tenant.Phone;
            tenant.Status = request.Status ?? tenant.Status;
            tenant.MaxUsers = request.MaxUsers ?? tenant.MaxUsers;
            tenant.MaxStorage = request.MaxStorage ?? tenant.MaxStorage;
            tenant.EnabledModules = request.EnabledModules ?? tenant.EnabledModules;
            tenant.Settings = request.Settings ?? tenant.Settings;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Success(MapToDto(tenant), "Tenant updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant {TenantId}", id);
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Deleta um tenant (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound(ErrorMessages.TenantNotFound);

            tenant.Status = TenantStatus.Deleted;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Success("Tenant deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant {TenantId}", id);
            return Error(ErrorMessages.InternalServerError);
        }
    }

    private TenantDto MapToDto(ERP.Master.Models.Tenant t) => new TenantDto
    {
        Id = t.Id,
        Name = t.Name,
        Cnpj = t.Cnpj,
        Email = t.Email,
        Phone = t.Phone,
        Status = t.Status,
        DbName = t.DbName,
        MaxUsers = t.MaxUsers,
        MaxStorage = t.MaxStorage,
        CurrentStorage = t.CurrentStorage,
        TrialEnd = t.TrialEnd,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt,
        Subscription = t.Subscription != null ? new SubscriptionDto
        {
            Id = t.Subscription.Id,
            PlanId = t.Subscription.PlanId,
            PlanName = t.Subscription.Plan?.Name,
            Status = t.Subscription.Status,
            StartDate = t.Subscription.StartDate,
            EndDate = t.Subscription.EndDate,
            TrialEnd = t.Subscription.TrialEnd
        } : null
    };

    private TenantDetailsDto MapToDetailsDto(ERP.Master.Models.Tenant t) => new TenantDetailsDto
    {
        Id = t.Id,
        Name = t.Name,
        Cnpj = t.Cnpj,
        Email = t.Email,
        Phone = t.Phone,
        Status = t.Status,
        DbName = t.DbName,
        ConnectionString = t.ConnectionString,
        MaxUsers = t.MaxUsers,
        MaxStorage = t.MaxStorage,
        CurrentStorage = t.CurrentStorage,
        TrialEnd = t.TrialEnd,
        EnabledModules = t.EnabledModules,
        Settings = t.Settings,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt,
        Subscription = t.Subscription != null ? new SubscriptionDto
        {
            Id = t.Subscription.Id,
            PlanId = t.Subscription.PlanId,
            PlanName = t.Subscription.Plan?.Name,
            Status = t.Subscription.Status,
            StartDate = t.Subscription.StartDate,
            EndDate = t.Subscription.EndDate,
            TrialStart = t.Subscription.TrialStart,
            TrialEnd = t.Subscription.TrialEnd,
            PaymentMethod = t.Subscription.PaymentMethod,
            PaymentStatus = t.Subscription.PaymentStatus,
            MaxUsers = t.Subscription.MaxUsers,
            MaxFilials = t.Subscription.MaxFilials,
            MaxStorage = t.Subscription.MaxStorage
        } : null,
        Users = t.Users.Select(u => new TenantUserDto
        {
            Id = u.Id,
            Username = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Status = u.Status,
            TwoFactorEnabled = u.TwoFactorEnabled,
            LastLogin = u.LastLogin
        }).ToList()
    };

    private string GetTenantConnectionString(string dbName) =>
        "Host=erp-db;Port=5432;Database=" + dbName + ";Username=postgres;Password=postgres";

    private async Task CreateTenantAdminUserAsync(ERP.Master.Models.Tenant tenant, CreateTenantUserRequest adminUser)
    {
        // Implementation for creating admin user
    }

    private async Task CreateTenantSubscriptionAsync(ERP.Master.Models.Tenant tenant, CreateSubscriptionRequest subscription)
    {
        // Implementation for creating subscription
    }
}
