using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly TenantDbContext _db;

    public ClientsController(TenantDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var clients = await _db.Clients.AsNoTracking().Include(c => c.Person)
            .OrderBy(c => c.Person.Name).Select(c => ToResponse(c)).ToListAsync(cancellationToken);
        return Ok(clients);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create([FromBody] ClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await _db.People.AnyAsync(p => p.Email == request.Email && p.IsActive, cancellationToken))
            return Conflict("Já existe um cliente ativo com este e-mail.");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var person = new Person { Name = request.Name.Trim(), Email = request.Email.Trim(), Phone = request.Phone?.Trim(), IsActive = true };
        var client = new Client { Person = person, Code = string.IsNullOrWhiteSpace(request.Code) ? $"CLI-{Guid.NewGuid():N}"[..12].ToUpperInvariant() : request.Code.Trim(), IsActive = true };
        _db.Add(client);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, ToResponse(client));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var client = await _db.Clients.AsNoTracking().Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return client is null ? NotFound() : Ok(ToResponse(client));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> Update(Guid id, [FromBody] ClientRequest request, CancellationToken cancellationToken)
    {
        var client = await _db.Clients.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (client is null) return NotFound();
        client.Person.Name = request.Name.Trim(); client.Person.Email = request.Email.Trim(); client.Person.Phone = request.Phone?.Trim();
        client.Code = string.IsNullOrWhiteSpace(request.Code) ? client.Code : request.Code.Trim();
        client.UpdatedAt = DateTime.UtcNow; client.Person.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(client));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var client = await _db.Clients.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (client is null) return NotFound();
        client.IsActive = false; client.Person.IsActive = false; client.UpdatedAt = DateTime.UtcNow; client.Person.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClientResponse ToResponse(Client client) => new(client.Id, client.Code, client.Person.Name, client.Person.Email ?? "", client.Person.Phone ?? "", client.IsActive ? "Ativo" : "Pendente");
}

public sealed record ClientRequest(string Name, string Email, string? Phone, string? Company, string? Code = null);
public sealed record ClientResponse(Guid Id, string Code, string Name, string Email, string Phone, string Status);

[Authorize]
[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly TenantDbContext _db;

    public ProductsController(TenantDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _db.Products.AsNoTracking().OrderBy(p => p.Name).Select(p => ToResponse(p)).ToListAsync(cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var code = string.IsNullOrWhiteSpace(request.Code) ? request.Sku : request.Code;
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Nome e SKU são obrigatórios.");
        if (await _db.Products.AnyAsync(p => p.Code == code && p.IsActive, cancellationToken)) return Conflict("Já existe um produto ativo com este SKU.");
        var product = new Product { Code = code.Trim(), Name = request.Name.Trim(), SalePrice = request.Price, ProductType = ProductType.Product, UnitOfMeasure = "UN", IsActive = true, ManageStock = true, AllowSaleWithoutStock = false };
        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToResponse(product));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return product is null ? NotFound() : Ok(ToResponse(product));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null) return NotFound();
        product.IsActive = false; product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ProductResponse ToResponse(Product product) => new(product.Id, product.Code, product.Name, product.SalePrice ?? 0, product.IsActive);
}

public sealed record ProductRequest(string Name, string Sku, string? Category, decimal Price, int Stock, string? Code = null);
public sealed record ProductResponse(Guid Id, string Sku, string Name, decimal Price, bool Active);
