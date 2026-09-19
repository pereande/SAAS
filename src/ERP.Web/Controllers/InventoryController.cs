using System;
using System.Linq;
using System.Threading.Tasks;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using ERP.Web.Services;
using static ERP.Web.Controllers.InventoryDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Web.Controllers;

/// <summary>
/// Controller para estoque (módulo Estoque)
/// </summary>
public class InventoryController : BaseController
{
    private readonly TenantDbContext _db;
    private readonly InventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public InventoryController(TenantDbContext db, InventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _db = db;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Lista o estoque dos produtos do tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetInventoryRequest request)
    {
        var query = _db.Inventories
            .Include(i => i.Product)
            .Include(i => i.Branch)
            .AsQueryable();

        if (request.ProductId.HasValue)
            query = query.Where(i => i.ProductId == request.ProductId);

        if (request.BranchId.HasValue)
            query = query.Where(i => i.BranchId == request.BranchId);

        query = query.OrderBy(i => i.Product.Name);

        var totalCount = await query.CountAsync();
        var inventories = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Success(new PagedResponse<InventoryDto>
        {
            Data = inventories.Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductCode = i.Product.Code,
                ProductName = i.Product.Name,
                BranchId = i.BranchId,
                BranchName = i.Branch.Name,
                Quantity = i.Quantity,
                ReservedQuantity = i.ReservedQuantity,
                AvailableQuantity = i.AvailableQuantity,
                MinStock = i.Product.MinStock,
                Location = i.Location,
                LastEntryDate = i.LastEntryDate,
                LastExitDate = i.LastExitDate
            }),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Lista as movimentações de estoque do tenant
    /// </summary>
    [HttpGet("movements")]
    public async Task<IActionResult> GetMovements([FromQuery] GetMovementsRequest request)
    {
        var query = _db.InventoryMovements
            .Include(m => m.Product)
            .Include(m => m.Branch)
            .AsQueryable();

        if (request.ProductId.HasValue)
            query = query.Where(m => m.ProductId == request.ProductId);

        query = query.OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync();
        var movements = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Success(new PagedResponse<MovementDto>
        {
            Data = movements.Select(m => new MovementDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                ProductCode = m.Product.Code,
                ProductName = m.Product.Name,
                BranchName = m.Branch.Name,
                MovementType = (int)m.MovementType,
                Quantity = m.Quantity,
                PreviousQuantity = m.PreviousQuantity,
                NewQuantity = m.NewQuantity,
                Description = m.Description,
                CreatedAt = m.CreatedAt
            }),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Faz um ajuste manual de estoque (entrada ou saída)
    /// </summary>
    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust([FromBody] AdjustInventoryRequest request)
    {
        if (request.Quantity <= 0)
            return ValidationError(new Dictionary<string, List<string>>
                { { "quantity", new List<string> { "Quantity must be greater than zero" } } });

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
        if (product == null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "productId", new List<string> { "Product not found" } } });

        var branchId = request.BranchId ?? await _db.Branches
            .OrderBy(b => b.Code)
            .Select(b => (Guid?)b.Id)
            .FirstOrDefaultAsync();

        if (branchId == null)
            return ValidationError(new Dictionary<string, List<string>>
                { { "branchId", new List<string> { "No branch found for this tenant" } } });

        var isEntry = string.Equals(request.Type, "in", StringComparison.OrdinalIgnoreCase);
        var movementType = isEntry ? InventoryMovementType.AdjustmentIn : InventoryMovementType.AdjustmentOut;

        if (!isEntry && product.ManageStock &&
            !await _inventoryService.HasAvailableStockAsync(branchId.Value, request.ProductId, request.Quantity))
            return ValidationError(new Dictionary<string, List<string>>
                { { "quantity", new List<string> { "Insufficient stock for this adjustment" } } });

        await _inventoryService.ApplyMovementAsync(
            branchId.Value,
            request.ProductId,
            movementType,
            request.Quantity,
            string.IsNullOrWhiteSpace(request.Description) ? "Manual adjustment" : request.Description,
            userId: GetCurrentUsername());

        await _db.SaveChangesAsync();

        _logger.LogInformation("Inventory adjusted: product {ProductId}, {Type} {Quantity}",
            request.ProductId, request.Type, request.Quantity);
        return Success("Inventory adjusted successfully");
    }
}

/// <summary>
/// DTOs de estoque
/// </summary>
public static class InventoryDtos
{
    public class GetInventoryRequest
    {
        public Guid? ProductId { get; set; }
        public Guid? BranchId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class InventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal? MinStock { get; set; }
        public string? Location { get; set; }
        public DateTime? LastEntryDate { get; set; }
        public DateTime? LastExitDate { get; set; }
    }

    public class GetMovementsRequest
    {
        public Guid? ProductId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class MovementDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? BranchName { get; set; }
        public int MovementType { get; set; }
        public decimal Quantity { get; set; }
        public decimal? PreviousQuantity { get; set; }
        public decimal? NewQuantity { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdjustInventoryRequest
    {
        /// <summary>
        /// ID do produto
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// ID da filial (opcional, usa a matriz se não informado)
        /// </summary>
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Tipo do ajuste: "in" (entrada) ou "out" (saída)
        /// </summary>
        public string Type { get; set; } = "in";

        /// <summary>
        /// Quantidade
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Descrição do ajuste
        /// </summary>
        public string? Description { get; set; }
    }
}
