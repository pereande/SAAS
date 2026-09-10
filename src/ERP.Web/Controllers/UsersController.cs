using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Constants;
using ERP.Web.DTOs.Users;
using ERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public UsersController(MasterDbContext dbContext, UserManager<User> userManager, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista os papéis disponíveis com seus níveis de acesso
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var roles = await _dbContext.Roles
                .OrderByDescending(r => r.Level)
                .ToListAsync();

            return Success(roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Level = r.Level
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Cria um novo usuário. Sem papéis informados, recebe automaticamente o papel básico "User".
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            // Resolver papéis: informados ou o papel básico padrão
            var roleIds = (request.RoleIds ?? new List<Guid>()).Distinct().ToList();
            var roles = await _dbContext.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            if (roleIds.Count > 0 && roles.Count != roleIds.Count)
                return Error("Um ou mais papéis informados não existem.");

            if (roles.Count == 0)
            {
                // Novas contas sem papéis informados recebem automaticamente o papel básico
                var basicRole = await _dbContext.Roles
                    .FirstOrDefaultAsync(r => r.NormalizedName == RoleSeeder.BasicRoleName.ToUpper());

                if (basicRole == null)
                    return Error("Papel básico padrão não encontrado. Execute o seed de papéis.");

                roles.Add(basicRole);
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                TenantId = request.TenantId,
                Status = UserStatus.Active
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());
                return ValidationError(errors);
            }

            foreach (var role in roles)
            {
                _dbContext.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            }
            await _dbContext.SaveChangesAsync();

            var created = await _dbContext.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Tenant).ThenInclude(t => t.Subscription).ThenInclude(s => s.Plan)
                .FirstAsync(u => u.Id == user.Id);

            _logger.LogInformation("Usuário {UserId} criado com papéis: {Roles}", user.Id, string.Join(", ", roles.Select(r => r.Name)));

            return Success(MapToDto(created), "Usuário criado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Error(ErrorMessages.InternalServerError);
        }
    }

    /// <summary>
    /// Define os papéis de um usuário (substitui os papéis atuais)
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    public async Task<IActionResult> UpdateRoles(Guid id, [FromBody] UpdateUserRolesRequest request)
    {
        try
        {
            var distinctIds = request.RoleIds.Distinct().ToList();

            if (distinctIds.Count == 0)
                return Error("O usuário deve ter pelo menos um papel.");

            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("Usuário não encontrado");

            var roles = await _dbContext.Roles
                .Where(r => distinctIds.Contains(r.Id))
                .ToListAsync();

            if (roles.Count != distinctIds.Count)
                return Error("Um ou mais papéis informados não existem.");

            // Impedir que o admin remova o próprio papel Admin (evita lockout)
            var currentUserId = GetCurrentUserId();
            if (user.Id == currentUserId)
            {
                var hadAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");
                if (hadAdmin && !roles.Any(r => r.Name == "Admin"))
                    return Error("Você não pode remover seu próprio papel de Admin.");
            }

            var rolesToRemove = user.UserRoles
                .Where(ur => !distinctIds.Contains(ur.RoleId))
                .ToList();
            _dbContext.UserRoles.RemoveRange(rolesToRemove);

            var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            foreach (var roleId in distinctIds.Where(rid => !currentRoleIds.Contains(rid)))
            {
                _dbContext.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Papéis do usuário {UserId} atualizados para: {Roles}", user.Id,
                string.Join(", ", roles.Select(r => r.Name)));

            var updated = await _dbContext.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Tenant).ThenInclude(t => t.Subscription).ThenInclude(s => s.Plan)
                .FirstAsync(u => u.Id == user.Id);

            return Success(MapToDto(updated), "Papéis atualizados com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user roles");
            return Error(ErrorMessages.InternalServerError);
        }
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
            RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList(),
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
