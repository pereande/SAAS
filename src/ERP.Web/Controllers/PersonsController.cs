using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Shared.Constants;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using static ERP.Web.Controllers.PersonDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para clientes e fornecedores (módulo Cadastros)
/// </summary>
public class PersonsController : BaseController
{
    private readonly TenantDbContext _db;
    private readonly ILogger<PersonsController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public PersonsController(TenantDbContext db, ILogger<PersonsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Lista clientes ou fornecedores do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPersonsRequest request)
    {
        var name = request.Name?.ToLower() ?? string.Empty;
        var isSupplier = string.Equals(request.Type, "supplier", StringComparison.OrdinalIgnoreCase);

        if (isSupplier)
        {
            var supplierQuery = _db.Suppliers.Include(s => s.Person).AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
                supplierQuery = supplierQuery.Where(s => s.Person.Name.ToLower().Contains(name));
            supplierQuery = supplierQuery.OrderBy(s => s.Person.Name);

            var totalSuppliers = await supplierQuery.CountAsync();
            var suppliers = await supplierQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return Success(new PagedResponse<PersonDto>
            {
                Data = suppliers.Select(s => MapToDto(s.Id, s.PersonId, s.Code, s.Person, "supplier", s.IsActive)),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalSuppliers
            });
        }
        else
        {
            var clientQuery = _db.Clients.Include(c => c.Person).AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
                clientQuery = clientQuery.Where(c => c.Person.Name.ToLower().Contains(name));
            clientQuery = clientQuery.OrderBy(c => c.Person.Name);

            var totalClients = await clientQuery.CountAsync();
            var clients = await clientQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return Success(new PagedResponse<PersonDto>
            {
                Data = clients.Select(c => MapToDto(c.Id, c.PersonId, c.Code, c.Person, "client", c.IsActive)),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalClients
            });
        }
    }

    /// <summary>
    /// Cria um cliente ou fornecedor
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return ValidationError(new Dictionary<string, List<string>>
                { { "name", new List<string> { "Name is required" } } });

        var isSupplier = string.Equals(request.Type, "supplier", StringComparison.OrdinalIgnoreCase);

        var person = new Person
        {
            TenantId = _db.TenantId,
            Name = request.Name,
            Document = request.Document,
            Email = request.Email,
            Phone = request.Phone
        };
        await _db.People.AddAsync(person);
        await _db.SaveChangesAsync();

        var count = isSupplier
            ? await _db.Suppliers.CountAsync()
            : await _db.Clients.CountAsync();
        var code = $"{(isSupplier ? "FR" : "CL")}{count + 1:0000}";

        if (isSupplier)
        {
            var supplier = new Supplier
            {
                TenantId = _db.TenantId,
                PersonId = person.Id,
                Code = code
            };
            await _db.Suppliers.AddAsync(supplier);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Supplier created: {SupplierId}", supplier.Id);
            return Success(MapToDto(supplier.Id, person.Id, supplier.Code, person, "supplier", true), "Supplier created successfully");
        }
        else
        {
            var client = new Client
            {
                TenantId = _db.TenantId,
                PersonId = person.Id,
                Code = code
            };
            await _db.Clients.AddAsync(client);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Client created: {ClientId}", client.Id);
            return Success(MapToDto(client.Id, person.Id, client.Code, person, "client", true), "Client created successfully");
        }
    }

    /// <summary>
    /// Desativa um cliente ou fornecedor (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string type = "client")
    {
        var isSupplier = string.Equals(type, "supplier", StringComparison.OrdinalIgnoreCase);

        if (isSupplier)
        {
            var supplier = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (supplier == null)
                return NotFound("Supplier not found");
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
                return NotFound("Client not found");
            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return Success("Person deactivated successfully");
    }

    private static PersonDto MapToDto(Guid id, Guid personId, string code, Person person, string type, bool isActive) =>
        new PersonDto
        {
            Id = id,
            PersonId = personId,
            Code = code,
            Name = person.Name,
            Document = person.Document,
            Email = person.Email,
            Phone = person.Phone,
            Type = type,
            IsActive = isActive
        };
}

/// <summary>
/// DTOs de pessoas
/// </summary>
public static class PersonDtos
{
    public class GetPersonsRequest
    {
        /// <summary>
        /// Tipo: "client" ou "supplier"
        /// </summary>
        public string Type { get; set; } = "client";
        public string Name { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PersonDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Type { get; set; } = "client";
        public bool IsActive { get; set; } = true;
    }

    public class CreatePersonRequest
    {
        /// <summary>
        /// Tipo: "client" ou "supplier"
        /// </summary>
        public string Type { get; set; } = "client";
        public string Name { get; set; } = string.Empty;
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
