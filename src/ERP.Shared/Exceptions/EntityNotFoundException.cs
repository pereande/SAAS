using System;

namespace ERP.Shared.Exceptions;

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo do ID</typeparam>
public class EntityNotFoundException<TEntity, TKey> : NotFoundException
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="id">ID da entidade não encontrada</param>
    public EntityNotFoundException(TKey id)
        : base($"{typeof(TEntity).Name} with ID '{id}' was not found.")
    {
        EntityType = typeof(TEntity).Name;
        EntityId = id?.ToString();
    }

    /// <summary>
    /// Construtor com mensagem personalizada
    /// </summary>
    /// <param name="id">ID da entidade não encontrada</param>
    /// <param name="message">Mensagem personalizada</param>
    public EntityNotFoundException(TKey id, string message)
        : base(message)
    {
        EntityType = typeof(TEntity).Name;
        EntityId = id?.ToString();
    }

    /// <summary>
    /// Tipo da entidade
    /// </summary>
    public string? EntityType { get; }

    /// <summary>
    /// ID da entidade
    /// </summary>
    public string? EntityId { get; }
}

/// <summary>
/// Exceção base para recursos não encontrados
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Construtor com inner exception
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Inner exception</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
