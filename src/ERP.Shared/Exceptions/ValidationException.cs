using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Shared.Exceptions;

/// <summary>
/// Exceção lançada quando há erros de validação
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="errors">Dicionário de erros (chave: nome do campo, valor: lista de mensagens)</param>
    public ValidationException(Dictionary<string, List<string>> errors)
        : base(BuildErrorMessage(errors))
    {
        Errors = errors ?? new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Construtor com mensagem e erros
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="errors">Dicionário de erros</param>
    public ValidationException(string message, Dictionary<string, List<string>> errors)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Erros de validação
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; }

    /// <summary>
    /// Adiciona um erro
    /// </summary>
    /// <param name="propertyName">Nome da propriedade</param>
    /// <param name="errorMessage">Mensagem de erro</param>
    public void AddError(string propertyName, string errorMessage)
    {
        if (!Errors.ContainsKey(propertyName))
        {
            Errors[propertyName] = new List<string>();
        }
        Errors[propertyName].Add(errorMessage);
    }

    /// <summary>
    /// Adiciona vários erros
    /// </summary>
    /// <param name="errors">Dicionário de erros</param>
    public void AddErrors(Dictionary<string, List<string>> errors)
    {
        foreach (var kvp in errors)
        {
            if (!Errors.ContainsKey(kvp.Key))
            {
                Errors[kvp.Key] = new List<string>();
            }
            Errors[kvp.Key].AddRange(kvp.Value);
        }
    }

    /// <summary>
    /// Constrói mensagem de erro a partir do dicionário
    /// </summary>
    /// <param name="errors">Dicionário de erros</param>
    /// <returns>Mensagem de erro formatada</returns>
    private static string BuildErrorMessage(Dictionary<string, List<string>> errors)
    {
        var errorMessages = errors
            .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
            .ToList();

        return string.Join(" | ", errorMessages);
    }
}
