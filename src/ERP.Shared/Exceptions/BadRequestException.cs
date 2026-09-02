using System;

namespace ERP.Shared.Exceptions;

/// <summary>
/// Exceção lançada quando há uma requisição inválida
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public BadRequestException(string message = "Bad request.")
        : base(message)
    {
    }

    /// <summary>
    /// Construtor com inner exception
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Inner exception</param>
    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
