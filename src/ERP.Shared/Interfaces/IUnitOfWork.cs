using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Shared.Interfaces;

/// <summary>
/// Interface para Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Confirma todas as alterações no banco de dados
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Número de linhas afetadas</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz todas as alterações não confirmadas
    /// </summary>
    void Rollback();

    /// <summary>
    /// Inicia uma transação
    /// </summary>
    /// <returns>Transação</returns>
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface para transação do banco de dados
/// </summary>
public interface IDbTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Confirma a transação
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
