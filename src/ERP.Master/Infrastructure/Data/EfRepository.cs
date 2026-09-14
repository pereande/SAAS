using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Shared.Interfaces;
using ERP.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Master.Infrastructure.Data;

/// <summary>
/// Implementação EF Core do repositório para entidades persistidas no banco mestre.
/// </summary>
public sealed class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly MasterDbContext _dbContext;
    private readonly DbSet<TEntity> _set;

    public EfRepository(MasterDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) =>
        await _set.FindAsync(new object?[] { id }, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _set.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await _set.Where(predicate).ToListAsync(cancellationToken);

    public Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<TEntity>> AddRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await _set.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        _set.UpdateRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        _set.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entities = await _set.Where(predicate).ToListAsync(cancellationToken);
        _set.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate == null ? _set.CountAsync(cancellationToken) : _set.CountAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _set.AnyAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default) =>
        _set.AnyAsync(entity => entity.Id.Equals(id), cancellationToken);
}
