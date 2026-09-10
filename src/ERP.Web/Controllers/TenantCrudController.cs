using ERP.Shared.Models;
using ERP.Tenant.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers;

/// <summary>
/// Generic CRUD base controller for tenant-scoped entities.
/// Provides standard list (paginated + search), get-by-id, create, update, and delete.
/// The <see cref="TenantDbContext"/> is resolved per-request from the current tenant.
/// </summary>
/// <typeparam name="TEntity">Entity type that extends <see cref="BaseEntity{Guid}"/> and implements <see cref="ITenantEntity"/>.</typeparam>
public abstract class TenantCrudController<TEntity> : BaseController
    where TEntity : BaseEntity<Guid>, ITenantEntity, new()
{
    /// <summary>
    /// DbContext do tenant atual
    /// </summary>
    protected readonly TenantDbContext DbContext;

    /// <summary>
    /// DbSet da entidade
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Constructor
    /// </summary>
    protected TenantCrudController(TenantDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Lista todas as entidades com paginação e busca opcional
    /// </summary>
    [HttpGet]
    public virtual async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = ApplySearch(query, search);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Success(new PagedResponse<TEntity>
        {
            Data = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Obtém uma entidade pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public virtual async Task<IActionResult> GetById(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            return NotFound($"{typeof(TEntity).Name} not found.");

        return Success(entity);
    }

    /// <summary>
    /// Cria uma nova entidade
    /// </summary>
    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        entity.Id = Guid.NewGuid();
        entity.TenantId = DbContext.TenantId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.CreatedBy = GetCurrentUserId().ToString();

        await DbSet.AddAsync(entity);
        await DbContext.SaveChangesAsync();

        return Success(entity, $"{typeof(TEntity).Name} created successfully.");
    }

    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public virtual async Task<IActionResult> Update(Guid id, [FromBody] TEntity entity)
    {
        var existing = await DbSet.FindAsync(id);
        if (existing == null)
            return NotFound($"{typeof(TEntity).Name} not found.");

        // Preserve immutable fields
        var tenantId = existing.TenantId;
        var createdAt = existing.CreatedAt;
        var createdBy = existing.CreatedBy;

        DbContext.Entry(existing).CurrentValues.SetValues(entity);
        existing.Id = id;
        existing.TenantId = tenantId;
        existing.CreatedAt = createdAt;
        existing.CreatedBy = createdBy;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentUserId().ToString();

        await DbContext.SaveChangesAsync();

        return Success(existing, $"{typeof(TEntity).Name} updated successfully.");
    }

    /// <summary>
    /// Remove uma entidade
    /// </summary>
    [HttpDelete("{id:guid}")]
    public virtual async Task<IActionResult> Delete(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            return NotFound($"{typeof(TEntity).Name} not found.");

        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync();

        return Success($"{typeof(TEntity).Name} deleted successfully.");
    }

    /// <summary>
    /// Applies a search filter to the query.
    /// By default, filters by the <c>Name</c> property if the entity has one.
    /// Override in specific controllers for custom search logic.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string search)
    {
        var nameProp = typeof(TEntity).GetProperty("Name");
        if (nameProp is not null && nameProp.PropertyType == typeof(string))
            return query.Where(e => EF.Property<string>(e, "Name")!.Contains(search));

        return query;
    }
}
