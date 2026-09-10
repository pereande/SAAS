using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Shared.Constants;
using ERP.Web.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para listagem de usuários no painel administrativo
/// </summary>
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : BaseController
{
    private readonly MasterDbContext _dbContext;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public UsersController(MasterDbContext dbContext, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Lista usuários com papéis e status da assinatura do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersRequest request)
    {
        try
        {
            var query = _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.Tenant)
                .ThenInclude(t => t.Subscription)
                .ThenInclude(s => s.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.UserName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == request.Role));

            if (request.Status.HasValue)
                query = query.Where(u => u.Status == request.Status);

            query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var response = new PagedResponse<UserDto>
            {
                Data = users.Select(MapToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Mapeia a entidade User para o DTO do painel
    /// </summary>
    private static UserDto MapToDto(ERP.Master.Models.User user)
    {
        var subscription = user.Tenant?.Subscription;

        return new UserDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            UserName = user.UserName,
            Email = user.Email,
            EmailVerified = user.EmailVerified,
            Status = user.Status,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).OrderBy(r => r).ToList(),
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt,
            Tenant = user.Tenant == null ? null : new UserTenantDto
            {
                Id = user.Tenant.Id,
                Name = user.Tenant.Name,
                Status = user.Tenant.Status,
                PlanName = subscription?.Plan?.Name,
                SubscriptionStatus = subscription?.Status,
                SubscriptionEndDate = subscription?.EndDate
            }
        };
    }
}
