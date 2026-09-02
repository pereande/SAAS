using System;
using System.Security.Claims;
using ERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller base com funcionalidades comuns
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Obtém o ID do usuário autenticado
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                          User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException(ErrorMessages.Unauthorized);
        }

        return userId;
    }

    /// <summary>
    /// Obtém o ID do tenant atual
    /// </summary>
    protected Guid? GetCurrentTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id");

        if (tenantIdClaim == null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return null;
        }

        return tenantId;
    }

    /// <summary>
    /// Obtém o nome do usuário autenticado
    /// </summary>
    protected string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ??
               User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value ??
               "Unknown";
    }

    /// <summary>
    /// Obtém o e-mail do usuário autenticado
    /// </summary>
    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ??
               User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ??
               "unknown@erpsaas.com";
    }

    /// <summary>
    /// Obtém os roles do usuário autenticado
    /// </summary>
    protected List<string> GetCurrentUserRoles()
    {
        return User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    /// <summary>
    /// Verifica se o usuário tem um role específico
    /// </summary>
    /// <param name="role">Role a verificar</param>
    /// <returns>True se o usuário tiver o role</returns>
    protected bool HasRole(string role)
    {
        return User.IsInRole(role);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica
    /// </summary>
    /// <param name="permission">Permissão a verificar</param>
    /// <returns>True se o usuário tiver a permissão</returns>
    protected bool HasPermission(string permission)
    {
        return User.HasClaim(c => c.Type == "permission" && c.Value == permission);
    }

    /// <summary>
    /// Retorna uma resposta de sucesso
    /// </summary>
    /// <typeparam name="T">Tipo do dado</typeparam>
    /// <param name="data">Dados a retornar</param>
    /// <param name="message">Mensagem de sucesso</param>
    /// <returns>OkObjectResult</returns>
    protected IActionResult Success<T>(T data, string message = "Operation completed successfully")
    {
        return Ok(new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Retorna uma resposta de sucesso sem dados
    /// </summary>
    /// <param name="message">Mensagem de sucesso</param>
    /// <returns>OkObjectResult</returns>
    protected IActionResult Success(string message = "Operation completed successfully")
    {
        return Ok(new ApiResponse
        {
            Success = true,
            Message = message
        });
    }

    /// <summary>
    /// Retorna uma resposta de erro
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="errorCode">Código de erro</param>
    /// <returns>BadRequestObjectResult</returns>
    protected IActionResult Error(string message, string errorCode = "BAD_REQUEST")
    {
        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        });
    }

    /// <summary>
    /// Retorna uma resposta de erro com validação
    /// </summary>
    /// <param name="errors">Dicionário de erros</param>
    /// <returns>BadRequestObjectResult</returns>
    protected IActionResult ValidationError(Dictionary<string, List<string>> errors)
    {
        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Validation failed",
            ErrorCode = "VALIDATION_ERROR",
            Errors = errors
        });
    }

    /// <summary>
    /// Retorna uma resposta de não encontrado
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <returns>NotFoundObjectResult</returns>
    protected IActionResult NotFound(string message = "Resource not found")
    {
        return NotFound(new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "NOT_FOUND"
        });
    }

    /// <summary>
    /// Retorna uma resposta de não autorizado
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <returns>UnauthorizedObjectResult</returns>
    protected IActionResult Unauthorized(string message = "Unauthorized access")
    {
        return Unauthorized(new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "UNAUTHORIZED"
        });
    }

    /// <summary>
    /// Retorna uma resposta de acesso negado
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <returns>ForbidResult</returns>
    protected IActionResult Forbidden(string message = "Forbidden: You do not have permission")
    {
        return Forbid(new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = "FORBIDDEN"
        });
    }
}

/// <summary>
/// Resposta genérica da API
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Sucesso
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Código de erro
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Erros de validação
    /// </summary>
    public Dictionary<string, List<string>>? Errors { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Resposta da API com dados
/// </summary>
/// <typeparam name="T">Tipo do dado</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Dados
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// Resposta paginada da API
/// </summary>
/// <typeparam name="T">Tipo do dado</typeparam>
public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>
    /// Página atual
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Tem página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Tem próxima página
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
