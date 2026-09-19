using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Shared.Constants;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using static ERP.Web.Controllers.ProductDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para produtos (módulo Estoque)
/// </summary>
public class ProductsController : BaseController
{
    private readonly TenantDbContext _db;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public ProductsController(TenantDbContext db, ILogger<ProductsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Lista os produtos do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsRequest request)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Brand).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(p => p.Name.ToLower().Contains(request.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.Code))
            query = query.Where(p => p.Code == request.Code);

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive);

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var response = new PagedResponse<ProductDto>
        {
            Data = products.Select(MapToDto),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Success(response);
    }

    /// <summary>
    /// Obtém um produto pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(ErrorMessages.ProductNotFound);

        return Success(MapToDto(product));
    }

    /// <summary>
    /// Cria um produto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return ValidationError(new Dictionary<string, List<string>>
                { { "name", new List<string> { "Name is required" } } });

        if (string.IsNullOrWhiteSpace(request.Code))
            request.Code = $"PRD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        if (await _db.Products.AnyAsync(p => p.Code == request.Code))
            return ValidationError(new Dictionary<string, List<string>>
                { { "code", new List<string> { "Product code already exists" } } });

        var product = new Product
        {
            TenantId = _db.TenantId,
            Code = request.Code,
            Barcode = request.Barcode,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            ProductType = request.ProductType,
            UnitOfMeasure = string.IsNullOrWhiteSpace(request.UnitOfMeasure) ? "UN" : request.UnitOfMeasure,
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            MinStock = request.MinStock,
            MaxStock = request.MaxStock,
            ManageStock = request.ManageStock,
            IsActive = request.IsActive,
            Notes = request.Notes
        };

        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Product created: {ProductId} - {Code}", product.Id, product.Code);
        return Success(MapToDto(product), "Product created successfully");
    }

    /// <summary>
    /// Atualiza um produto
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound(ErrorMessages.ProductNotFound);

        if (string.IsNullOrWhiteSpace(request.Name))
            return ValidationError(new Dictionary<string, List<string>>
                { { "name", new List<string> { "Name is required" } } });

        if (!string.IsNullOrWhiteSpace(request.Code) && request.Code != product.Code &&
            await _db.Products.AnyAsync(p => p.Code == request.Code && p.Id != id))
            return ValidationError(new Dictionary<string, List<string>>
                { { "code", new List<string> { "Product code already exists" } } });

        product.Code = string.IsNullOrWhiteSpace(request.Code) ? product.Code : request.Code;
        product.Barcode = request.Barcode ?? product.Barcode;
        product.Name = request.Name;
        product.Description = request.Description ?? product.Description;
        product.CategoryId = request.CategoryId ?? product.CategoryId;
        product.BrandId = request.BrandId ?? product.BrandId;
        product.ProductType = request.ProductType;
        product.UnitOfMeasure = string.IsNullOrWhiteSpace(request.UnitOfMeasure) ? product.UnitOfMeasure : request.UnitOfMeasure;
        product.CostPrice = request.CostPrice ?? product.CostPrice;
        product.SalePrice = request.SalePrice ?? product.SalePrice;
        product.MinStock = request.MinStock ?? product.MinStock;
        product.MaxStock = request.MaxStock ?? product.MaxStock;
        product.ManageStock = request.ManageStock;
        product.IsActive = request.IsActive;
        product.Notes = request.Notes ?? product.Notes;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Success(MapToDto(product), "Product updated successfully");
    }

    /// <summary>
    /// Desativa um produto (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound(ErrorMessages.ProductNotFound);

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Success("Product deactivated successfully");
    }

    private static ProductDto MapToDto(Product p) => new ProductDto
    {
        Id = p.Id,
        Code = p.Code,
        Barcode = p.Barcode,
        Name = p.Name,
        Description = p.Description,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        ProductType = (int)p.ProductType,
        UnitOfMeasure = p.UnitOfMeasure,
        CostPrice = p.CostPrice,
        SalePrice = p.SalePrice,
        MinStock = p.MinStock,
        MaxStock = p.MaxStock,
        ManageStock = p.ManageStock,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}

/// <summary>
/// DTOs de produtos
/// </summary>
public static class ProductDtos
{
    public class GetProductsRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public int ProductType { get; set; }
        public string UnitOfMeasure { get; set; } = "UN";
        public decimal? CostPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? MinStock { get; set; }
        public decimal? MaxStock { get; set; }
        public bool ManageStock { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductRequest
    {
        public string? Code { get; set; }
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public ProductType ProductType { get; set; } = ProductType.Product;
        public string UnitOfMeasure { get; set; } = "UN";
        public decimal? CostPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? MinStock { get; set; }
        public decimal? MaxStock { get; set; }
        public bool ManageStock { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
    }

    public class UpdateProductRequest : CreateProductRequest
    {
    }
}
