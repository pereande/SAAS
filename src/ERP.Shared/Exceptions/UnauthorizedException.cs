using System;

namespace ERP.Shared.Exceptions;

/// <summary>
/// Exceção lançada quando o usuário não está autorizado
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public UnauthorizedException(string message = "Unauthorized access.")
        : base(message)
    {
    }

    /// <summary>
    /// Construtor com inner exception
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Inner exception</param>
    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção lançada quando o usuário não tem permissão
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public ForbiddenException(string message = "Forbidden: You do not have permission to perform this action.")
        : base(message)
    {
    }

    /// <summary>
    /// Construtor com inner exception
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Inner exception</param>
    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
