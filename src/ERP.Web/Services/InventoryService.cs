using System;
using System.Threading.Tasks;
using ERP.Tenant.Infrastructure.Data;
using ERP.Tenant.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Services;

/// <summary>
/// Serviço de movimentação de estoque do tenant atual
/// </summary>
public class InventoryService
{
    private readonly TenantDbContext _db;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="db">DbContext do tenant</param>
    public InventoryService(TenantDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Verifica se há estoque disponível suficiente de um produto em uma filial
    /// </summary>
    /// <param name="branchId">ID da filial</param>
    /// <param name="productId">ID do produto</param>
    /// <param name="quantity">Quantidade necessária</param>
    /// <returns>True se houver estoque disponível</returns>
    public async Task<bool> HasAvailableStockAsync(Guid branchId, Guid productId, decimal quantity)
    {
        var inventory = await _db.Inventories
            .FirstOrDefaultAsync(i => i.BranchId == branchId && i.ProductId == productId);

        return inventory == null || inventory.AvailableQuantity >= quantity;
    }

    /// <summary>
    /// Registra uma movimentação de estoque e atualiza o saldo do produto na filial.
    /// A persistência fica a cargo do chamador (SaveChangesAsync).
    /// </summary>
    /// <param name="branchId">ID da filial</param>
    /// <param name="productId">ID do produto</param>
    /// <param name="movementType">Tipo de movimento (entrada ou saída)</param>
    /// <param name="quantity">Quantidade movimentada</param>
    /// <param name="description">Descrição da movimentação</param>
    /// <param name="saleId">ID da venda (se aplicável)</param>
    /// <param name="purchaseId">ID da compra (se aplicável)</param>
    /// <param name="userId">ID do usuário responsável</param>
    public async Task ApplyMovementAsync(
        Guid branchId,
        Guid productId,
        InventoryMovementType movementType,
        decimal quantity,
        string description,
        Guid? saleId = null,
        Guid? purchaseId = null,
        string? userId = null)
    {
        var isEntry = movementType is InventoryMovementType.Entry
            or InventoryMovementType.AdjustmentIn
            or InventoryMovementType.TransferIn
            or InventoryMovementType.SalesReturn
            or InventoryMovementType.PurchaseReturn;

        var inventory = await _db.Inventories
            .FirstOrDefaultAsync(i => i.BranchId == branchId && i.ProductId == productId);

        var previousQuantity = inventory?.Quantity ?? 0;
        var newQuantity = isEntry ? previousQuantity + quantity : previousQuantity - quantity;

        if (inventory == null)
        {
            inventory = new Inventory
            {
                TenantId = _db.TenantId,
                BranchId = branchId,
                ProductId = productId,
                Quantity = 0
            };
            await _db.Inventories.AddAsync(inventory);
        }

        inventory.Quantity = newQuantity;
        if (isEntry)
        {
            inventory.LastEntryDate = DateTime.UtcNow;
        }
        else
        {
            inventory.LastExitDate = DateTime.UtcNow;
        }

        await _db.InventoryMovements.AddAsync(new InventoryMovement
        {
            TenantId = _db.TenantId,
            BranchId = branchId,
            ProductId = productId,
            InventoryId = inventory.Id,
            MovementType = movementType,
            Quantity = quantity,
            PreviousQuantity = previousQuantity,
            NewQuantity = newQuantity,
            Description = description,
            SaleId = saleId,
            PurchaseId = purchaseId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }
}
