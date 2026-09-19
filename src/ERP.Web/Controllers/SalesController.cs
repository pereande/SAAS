using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Shared.Constants;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using ERP.Web.Services;
using static ERP.Web.Controllers.SaleDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para vendas (módulo Vendas)
/// </summary>
public class SalesController : BaseController
{
    private readonly TenantDbContext _db;
    private readonly InventoryService _inventoryService;
    private readonly ILogger<SalesController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public SalesController(TenantDbContext db, InventoryService inventoryService, ILogger<SalesController> logger)
    {
        _db = db;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Lista as vendas do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSalesRequest request)
    {
        var query = _db.Sales.AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(s => s.Status == (SaleStatus)request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(s => s.Number.Contains(request.Number));

        if (request.ClientId.HasValue)
            query = query.Where(s => s.ClientId == request.ClientId);

        if (request.DateFrom.HasValue)
            query = query.Where(s => s.SaleDate >= request.DateFrom);

        if (request.DateTo.HasValue)
            query = query.Where(s => s.SaleDate < request.DateTo.Value.AddDays(1));

        query = query.OrderByDescending(s => s.SaleDate);

        var totalCount = await query.CountAsync();
        var sales = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Success(new PagedResponse<SaleDto>
        {
            Data = sales.Select(MapToDto),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Obtém uma venda pelo ID (com itens)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _db.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return NotFound("Sale not found");

        return Success(MapToDetailsDto(sale));
    }

    /// <summary>
    /// Cria uma venda (status Rascunho)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        var validationError = await GetItemValidationErrorAsync(request.Items);
        if (validationError != null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "items", new List<string> { validationError } } });

        var branchId = request.BranchId ?? await _db.Branches
            .OrderBy(b => b.Code)
            .Select(b => (Guid?)b.Id)
            .FirstOrDefaultAsync();

        if (branchId == null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "branchId", new List<string> { "No branch found for this tenant" } } });

        var products = await GetProductsAsync(request.Items);
        var sale = await BuildSaleAsync(request, branchId.Value, products);

        await _db.Sales.AddAsync(sale);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Sale created: {SaleId} - {Number}", sale.Id, sale.Number);
        return Success(MapToDetailsDto(sale), "Sale created successfully");
    }

    /// <summary>
    /// Atualiza uma venda (apenas rascunhos)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateSaleRequest request)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null)
            return NotFound("Sale not found");

        if (sale.Status != SaleStatus.Draft)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only draft sales can be updated" } } });

        if (request.Items == null || request.Items.Count == 0)
            return ValidationError(new Dictionary<string, List<string>>
                { { "items", new List<string> { "At least one item is required" } } });

        var validationError = await GetItemValidationErrorAsync(request.Items);
        if (validationError != null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "items", new List<string> { validationError } } });

        var products = await GetProductsAsync(request.Items);

        sale.BranchId = request.BranchId ?? sale.BranchId;
        sale.ClientId = request.ClientId;
        sale.ClientName = request.ClientName;
        sale.ClientDocument = request.ClientDocument;
        sale.SaleType = request.SaleType;
        sale.Discount = request.Discount;
        sale.Freight = request.Freight;
        sale.OtherExpenses = request.OtherExpenses;
        sale.Notes = request.Notes;
        sale.UpdatedAt = DateTime.UtcNow;

        _db.SaleItems.RemoveRange(sale.Items);
        await AddItemsAsync(sale, request.Items, products);
        RecalculateTotals(sale);

        await _db.SaveChangesAsync();
        return Success(MapToDetailsDto(sale), "Sale updated successfully");
    }

    /// <summary>
    /// Confirma uma venda (baixa estoque)
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null)
            return NotFound("Sale not found");

        if (sale.Status != SaleStatus.Draft)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only draft sales can be confirmed" } } });

        // Verificar estoque disponível dos produtos que controlam estoque
        var stockManagedProductIds = sale.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => stockManagedProductIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in sale.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product) ||
                !product.ManageStock || product.AllowSaleWithoutStock)
                continue;

            if (!await _inventoryService.HasAvailableStockAsync(sale.BranchId, item.ProductId, item.Quantity))
                return ValidationError(new Dictionary<string, List<string>>
                {
                    { "items", new List<string> { $"Insufficient stock for product {item.ProductName}" } }
                });
        }

        sale.Status = SaleStatus.Confirmed;
        sale.UpdatedAt = DateTime.UtcNow;

        foreach (var item in sale.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product) && product.ManageStock)
            {
                await _inventoryService.ApplyMovementAsync(
                    sale.BranchId, item.ProductId, InventoryMovementType.Exit, item.Quantity,
                    $"Sale {sale.Number}", saleId: sale.Id, userId: GetCurrentUsername());
            }
        }

        await _db.SaveChangesAsync();
        return Success(MapToDetailsDto(sale), "Sale confirmed successfully");
    }

    /// <summary>
    /// Cancela uma venda (devolve estoque se confirmada)
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelSaleRequest request)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null)
            return NotFound("Sale not found");

        if (sale.Status == SaleStatus.Cancelled)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Sale is already cancelled" } } });

        var wasConfirmed = sale.Status != SaleStatus.Draft;

        sale.Status = SaleStatus.Cancelled;
        sale.CancelledDate = DateTime.UtcNow;
        sale.CancelledReason = request?.Reason;
        sale.CancelledBy = GetCurrentUsername();
        sale.UpdatedAt = DateTime.UtcNow;

        if (wasConfirmed)
        {
            var productIds = sale.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in sale.Items)
            {
                if (products.TryGetValue(item.ProductId, out var product) && product.ManageStock)
                {
                    await _inventoryService.ApplyMovementAsync(
                        sale.BranchId, item.ProductId, InventoryMovementType.SalesReturn, item.Quantity,
                        $"Sale cancelled {sale.Number}", saleId: sale.Id, userId: GetCurrentUsername());
                }
            }
        }

        await _db.SaveChangesAsync();
        return Success(MapToDetailsDto(sale), "Sale cancelled successfully");
    }

    /// <summary>
    /// Exclui uma venda (apenas rascunhos)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null)
            return NotFound("Sale not found");

        if (sale.Status != SaleStatus.Draft)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only draft sales can be deleted" } } });

        _db.Sales.Remove(sale);
        await _db.SaveChangesAsync();
        return Success("Sale deleted successfully");
    }

    private async Task<string?> GetItemValidationErrorAsync(List<CreateSaleItemRequest> items)
    {
        if (items == null || items.Count == 0)
            return "At least one item is required";

        foreach (var item in items)
        {
            if (item.Quantity <= 0)
                return "Item quantity must be greater than zero";
        }

        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var foundCount = await _db.Products.CountAsync(p => productIds.Contains(p.Id));
        return foundCount != productIds.Count ? "One or more products were not found" : null;
    }

    private async Task<Dictionary<Guid, Product>> GetProductsAsync(List<CreateSaleItemRequest> items)
    {
        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        return await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
    }

    private async Task<Sale> BuildSaleAsync(CreateSaleRequest request, Guid branchId, Dictionary<Guid, Product> products)
    {
        var sale = new Sale
        {
            TenantId = _db.TenantId,
            BranchId = branchId,
            Number = $"V{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            ClientId = request.ClientId,
            ClientName = request.ClientName,
            ClientDocument = request.ClientDocument,
            SaleDate = DateTime.UtcNow,
            Status = SaleStatus.Draft,
            SaleType = request.SaleType,
            Discount = request.Discount,
            Freight = request.Freight,
            OtherExpenses = request.OtherExpenses,
            Notes = request.Notes
        };

        await AddItemsAsync(sale, request.Items, products);
        RecalculateTotals(sale);
        return sale;
    }

    private async Task AddItemsAsync(Sale sale, List<CreateSaleItemRequest> items, Dictionary<Guid, Product> products)
    {
        foreach (var item in items)
        {
            var product = products[item.ProductId];
            var unitPrice = item.UnitPrice ?? product.SalePrice ?? 0;
            var discount = item.Discount ?? 0;
            var lineSubtotal = item.Quantity * unitPrice;
            var lineTotal = lineSubtotal - discount;

            sale.Items.Add(new SaleItem
            {
                TenantId = _db.TenantId,
                ProductId = product.Id,
                ProductCode = product.Code,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitOfMeasure = product.UnitOfMeasure,
                UnitPrice = unitPrice,
                Discount = discount,
                Subtotal = lineSubtotal,
                Total = lineTotal
            });
        }

        await Task.CompletedTask;
    }

    private static void RecalculateTotals(Sale sale)
    {
        sale.Subtotal = sale.Items.Sum(i => i.Total);
        sale.Total = sale.Subtotal - sale.Discount + (sale.Freight ?? 0) + (sale.OtherExpenses ?? 0);
    }

    private static SaleDto MapToDto(Sale s) => new SaleDto
    {
        Id = s.Id,
        Number = s.Number,
        SaleDate = s.SaleDate,
        Status = (int)s.Status,
        SaleType = (int)s.SaleType,
        ClientId = s.ClientId,
        ClientName = s.ClientName,
        Subtotal = s.Subtotal,
        Discount = s.Discount,
        Total = s.Total,
        Notes = s.Notes,
        CreatedAt = s.CreatedAt
    };

    private static SaleDetailsDto MapToDetailsDto(Sale s)
    {
        var dto = new SaleDetailsDto
        {
            Id = s.Id,
            Number = s.Number,
            BranchId = s.BranchId,
            SaleDate = s.SaleDate,
            Status = (int)s.Status,
            SaleType = (int)s.SaleType,
            ClientId = s.ClientId,
            ClientName = s.ClientName,
            Subtotal = s.Subtotal,
            Discount = s.Discount,
            Freight = s.Freight,
            OtherExpenses = s.OtherExpenses,
            Total = s.Total,
            Notes = s.Notes,
            CancelledDate = s.CancelledDate,
            CancelledReason = s.CancelledReason,
            CreatedAt = s.CreatedAt
        };
        dto.Items = s.Items.Select(i => new SaleItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductCode = i.ProductCode,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitOfMeasure = i.UnitOfMeasure,
            UnitPrice = i.UnitPrice,
            Discount = i.Discount,
            Subtotal = i.Subtotal,
            Total = i.Total
        }).ToList();
        return dto;
    }
}

/// <summary>
/// DTOs de vendas
/// </summary>
public static class SaleDtos
{
    public class GetSalesRequest
    {
        public int? Status { get; set; }
        public string Number { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SaleDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public int Status { get; set; }
        public int SaleType { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SaleDetailsDto : SaleDto
    {
        public Guid BranchId { get; set; }
        public decimal? Freight { get; set; }
        public decimal? OtherExpenses { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateSaleRequest
    {
        public Guid? BranchId { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientDocument { get; set; }
        public SaleType SaleType { get; set; } = SaleType.Sale;
        public decimal Discount { get; set; }
        public decimal? Freight { get; set; }
        public decimal? OtherExpenses { get; set; }
        public string? Notes { get; set; }
        public List<CreateSaleItemRequest> Items { get; set; } = new List<CreateSaleItemRequest>();
    }

    public class CreateSaleItemRequest
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Discount { get; set; }
    }

    public class CancelSaleRequest
    {
        public string? Reason { get; set; }
    }
}
