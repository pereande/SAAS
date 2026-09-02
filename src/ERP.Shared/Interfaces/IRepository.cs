using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Shared.Models;

namespace ERP.Shared.Interfaces;

/// <summary>
/// Interface genérica para repositórios
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo do ID</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Obtém uma entidade pelo ID
    /// </summary>
    /// <param name="id">ID da entidade</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Entidade ou null</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as entidades
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de entidades</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém entidades com filtro
    /// </summary>
    /// <param name="predicate">Predicado de filtro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de entidades</returns>
    Task<IReadOnlyList<TEntity>> GetWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a primeira entidade que satisfaz o filtro
    /// </summary>
    /// <param name="predicate">Predicado de filtro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Entidade ou null</returns>
    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma entidade
    /// </summary>
    /// <param name="entity">Entidade a ser adicionada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Entidade adicionada</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona várias entidades
    /// </summary>
    /// <param name="entities">Entidades a serem adicionadas</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de entidades adicionadas</returns>
    Task<IReadOnlyList<TEntity>> AddRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade
    /// </summary>
    /// <param name="entity">Entidade a ser atualizada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza várias entidades
    /// </summary>
    /// <param name="entities">Entidades a serem atualizadas</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task UpdateRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade
    /// </summary>
    /// <param name="entity">Entidade a ser removida</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove várias entidades
    /// </summary>
    /// <param name="entities">Entidades a serem removidas</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task DeleteRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove entidades pelo filtro
    /// </summary>
    /// <param name="predicate">Predicado de filtro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task DeleteWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta entidades
    /// </summary>
    /// <param name="predicate">Predicado de filtro (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Número de entidades</returns>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe alguma entidade que satisfaz o filtro
    /// </summary>
    /// <param name="predicate">Predicado de filtro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se existir</returns>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe alguma entidade pelo ID
    /// </summary>
    /// <param name="id">ID da entidade</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se existir</returns>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}
