using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ERP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Middleware;

/// <summary>
/// Middleware para tratamento global de exceções
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="next">Próximo middleware</param>
    /// <param name="logger">Logger</param>
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Trata a exceção e retorna a resposta apropriada
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="exception">Exceção</param>
    /// <returns>Task</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Logar a exceção
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        // Criar resposta de erro
        var response = new
        {
            Success = false,
            Message = GetErrorMessage(exception),
            ErrorCode = GetErrorCode(exception),
            Errors = GetErrorDetails(exception),
            Timestamp = DateTime.UtcNow
        };

        // Definir status code
        context.Response.StatusCode = GetStatusCode(exception);
        context.Response.ContentType = "application/json";

        // Escrever resposta
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        }));
    }

    /// <summary>
    /// Obtém a mensagem de erro
    /// </summary>
    /// <param name="exception">Exceção</param>
    /// <returns>Mensagem de erro</returns>
    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException ve => ve.Message,
            BadRequestException bre => bre.Message,
            EntityNotFoundException _ => "Resource not found.",
            UnauthorizedException _ => "Unauthorized access.",
            ForbiddenException fe => fe.Message,
            _ => "An unexpected error occurred."
        };
    }

    /// <summary>
    /// Obtém o código de erro
    /// </summary>
    /// <param name="exception">Exceção</param>
    /// <returns>Código de erro</returns>
    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            ValidationException _ => "VALIDATION_ERROR",
            BadRequestException _ => "BAD_REQUEST",
            EntityNotFoundException _ => "NOT_FOUND",
            UnauthorizedException _ => "UNAUTHORIZED",
            ForbiddenException _ => "FORBIDDEN",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    /// <summary>
    /// Obtém os detalhes do erro
    /// </summary>
    /// <param name="exception">Exceção</param>
    /// <returns>Detalhes do erro</returns>
    private static object? GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException ve => ve.Errors,
            _ => null
        };
    }

    /// <summary>
    /// Obtém o status code HTTP
    /// </summary>
    /// <param name="exception">Exceção</param>
    /// <returns>Status code</returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException _ => (int)HttpStatusCode.BadRequest,
            BadRequestException _ => (int)HttpStatusCode.BadRequest,
            EntityNotFoundException _ => (int)HttpStatusCode.NotFound,
            UnauthorizedException _ => (int)HttpStatusCode.Unauthorized,
            ForbiddenException _ => (int)HttpStatusCode.Forbidden,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
}

/// <summary>
/// Extensions para ExceptionMiddleware
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de exceções
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
