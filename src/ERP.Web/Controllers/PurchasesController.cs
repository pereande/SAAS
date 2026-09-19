using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using ERP.Web.Services;
using static ERP.Web.Controllers.PurchaseDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para compras (módulo Compras)
/// </summary>
public class PurchasesController : BaseController
{
    private readonly TenantDbContext _db;
    private readonly InventoryService _inventoryService;
    private readonly ILogger<PurchasesController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public PurchasesController(TenantDbContext db, InventoryService inventoryService, ILogger<PurchasesController> logger)
    {
        _db = db;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Lista as compras do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPurchasesRequest request)
    {
        var query = _db.Purchases.AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(p => p.Status == (PurchaseStatus)request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(p => p.Number.Contains(request.Number));

        if (request.SupplierId.HasValue)
            query = query.Where(p => p.SupplierId == request.SupplierId);

        if (request.DateFrom.HasValue)
            query = query.Where(p => p.PurchaseDate >= request.DateFrom);

        if (request.DateTo.HasValue)
            query = query.Where(p => p.PurchaseDate < request.DateTo.Value.AddDays(1));

        query = query.OrderByDescending(p => p.PurchaseDate);

        var totalCount = await query.CountAsync();
        var purchases = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Success(new PagedResponse<PurchaseDto>
        {
            Data = purchases.Select(MapToDto),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Obtém uma compra pelo ID (com itens)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var purchase = await _db.Purchases
            .Include(p => p.Items)
            .Include(p => p.Supplier).ThenInclude(s => s.Person)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (purchase == null)
            return NotFound("Purchase not found");

        return Success(MapToDetailsDto(purchase));
    }

    /// <summary>
    /// Cria uma compra (status Rascunho)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseRequest request)
    {
        if (request.SupplierId == Guid.Empty)
            return ValidationError(new Dictionary<string, List<string>>
                { { "supplierId", new List<string> { "Supplier is required" } } });

        var validationError = await GetItemValidationErrorAsync(request.Items);
        if (validationError != null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "items", new List<string> { validationError } } });

        if (!await _db.Suppliers.AnyAsync(s => s.Id == request.SupplierId))
            return ValidationError(new Dictionary<string, List<string>>
                { { "supplierId", new List<string> { "Supplier not found" } } });

        var branchId = request.BranchId ?? await _db.Branches
            .OrderBy(b => b.Code)
            .Select(b => (Guid?)b.Id)
            .FirstOrDefaultAsync();

        if (branchId == null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "branchId", new List<string> { "No branch found for this tenant" } } });

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

        var purchase = new Purchase
        {
            TenantId = _db.TenantId,
            BranchId = branchId.Value,
            Number = $"C{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            SupplierId = request.SupplierId,
            PurchaseDate = DateTime.UtcNow,
            Status = PurchaseStatus.Draft,
            PurchaseType = request.PurchaseType,
            Discount = request.Discount,
            Freight = request.Freight,
            OtherExpenses = request.OtherExpenses,
            Notes = request.Notes
        };

        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            var unitPrice = item.UnitPrice ?? product.CostPrice ?? 0;
            var discount = item.Discount ?? 0;
            var lineSubtotal = item.Quantity * unitPrice;

            purchase.Items.Add(new PurchaseItem
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
                Total = lineSubtotal - discount
            });
        }

        purchase.Subtotal = purchase.Items.Sum(i => i.Total);
        purchase.Total = purchase.Subtotal - purchase.Discount + (purchase.Freight ?? 0) + (purchase.OtherExpenses ?? 0);

        await _db.Purchases.AddAsync(purchase);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Purchase created: {PurchaseId} - {Number}", purchase.Id, purchase.Number);
        return Success(MapToDetailsDto(purchase), "Purchase created successfully");
    }

    /// <summary>
    /// Confirma uma compra
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var purchase = await _db.Purchases.FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
            return NotFound("Purchase not found");

        if (purchase.Status != PurchaseStatus.Draft)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only draft purchases can be confirmed" } } });

        purchase.Status = PurchaseStatus.Confirmed;
        purchase.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Success(MapToDto(purchase), "Purchase confirmed successfully");
    }

    /// <summary>
    /// Recebe uma compra confirmada (dá entrada no estoque)
    /// </summary>
    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> Receive(Guid id)
    {
        var purchase = await _db.Purchases.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
            return NotFound("Purchase not found");

        if (purchase.Status != PurchaseStatus.Confirmed)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only confirmed purchases can be received" } } });

        purchase.Status = PurchaseStatus.Received;
        purchase.EntryDate = DateTime.UtcNow;
        purchase.UpdatedAt = DateTime.UtcNow;

        var productIds = purchase.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

        foreach (var item in purchase.Items)
        {
            if (item.ProductId == null)
                continue;

            if (products.TryGetValue(item.ProductId.Value, out var product) && product.ManageStock)
            {
                await _inventoryService.ApplyMovementAsync(
                    purchase.BranchId, item.ProductId.Value, InventoryMovementType.Entry, item.Quantity,
                    $"Purchase received {purchase.Number}", purchaseId: purchase.Id, userId: GetCurrentUsername());
            }
        }

        await _db.SaveChangesAsync();
        return Success(MapToDetailsDto(purchase), "Purchase received successfully");
    }

    /// <summary>
    /// Cancela uma compra
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelPurchaseRequest request)
    {
        var purchase = await _db.Purchases.FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
            return NotFound("Purchase not found");

        if (purchase.Status is PurchaseStatus.Cancelled or PurchaseStatus.Received)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "This purchase cannot be cancelled" } } });

        purchase.Status = PurchaseStatus.Cancelled;
        purchase.CancelledDate = DateTime.UtcNow;
        purchase.CancelledReason = request?.Reason;
        purchase.CancelledBy = GetCurrentUsername();
        purchase.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Success(MapToDto(purchase), "Purchase cancelled successfully");
    }

    /// <summary>
    /// Exclui uma compra (apenas rascunhos)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var purchase = await _db.Purchases.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
            return NotFound("Purchase not found");

        if (purchase.Status != PurchaseStatus.Draft)
            return ValidationError(new Dictionary<string, List<string>>
                { { "status", new List<string> { "Only draft purchases can be deleted" } } });

        _db.Purchases.Remove(purchase);
        await _db.SaveChangesAsync();
        return Success("Purchase deleted successfully");
    }

    private async Task<string?> GetItemValidationErrorAsync(List<CreatePurchaseItemRequest>? items)
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

    private static PurchaseDto MapToDto(Purchase p) => new PurchaseDto
    {
        Id = p.Id,
        Number = p.Number,
        PurchaseDate = p.PurchaseDate,
        Status = (int)p.Status,
        PurchaseType = (int)p.PurchaseType,
        SupplierId = p.SupplierId,
        Subtotal = p.Subtotal,
        Discount = p.Discount,
        Total = p.Total,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt
    };

    private static PurchaseDetailsDto MapToDetailsDto(Purchase p)
    {
        var dto = new PurchaseDetailsDto
        {
            Id = p.Id,
            Number = p.Number,
            BranchId = p.BranchId,
            PurchaseDate = p.PurchaseDate,
            Status = (int)p.Status,
            PurchaseType = (int)p.PurchaseType,
            SupplierId = p.SupplierId,
            Subtotal = p.Subtotal,
            Discount = p.Discount,
            Freight = p.Freight,
            OtherExpenses = p.OtherExpenses,
            Total = p.Total,
            Notes = p.Notes,
            CancelledDate = p.CancelledDate,
            CancelledReason = p.CancelledReason,
            CreatedAt = p.CreatedAt
        };
        dto.Items = p.Items.Select(i => new PurchaseItemDto
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
/// DTOs de compras
/// </summary>
public static class PurchaseDtos
{
    public class GetPurchasesRequest
    {
        public int? Status { get; set; }
        public string Number { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int Status { get; set; }
        public int PurchaseType { get; set; }
        public Guid SupplierId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseDetailsDto : PurchaseDto
    {
        public Guid BranchId { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? Freight { get; set; }
        public decimal? OtherExpenses { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new List<PurchaseItemDto>();
    }

    public class PurchaseItemDto
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }

    public class CreatePurchaseRequest
    {
        public Guid? BranchId { get; set; }
        public Guid SupplierId { get; set; }
        public PurchaseType PurchaseType { get; set; } = PurchaseType.Purchase;
        public decimal Discount { get; set; }
        public decimal? Freight { get; set; }
        public decimal? OtherExpenses { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseItemRequest> Items { get; set; } = new List<CreatePurchaseItemRequest>();
    }

    public class CreatePurchaseItemRequest
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Discount { get; set; }
    }

    public class CancelPurchaseRequest
    {
        public string? Reason { get; set; }
    }
}
