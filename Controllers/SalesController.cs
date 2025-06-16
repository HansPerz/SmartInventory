using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;
using SmartInventory.Models;

namespace SmartInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public SalesController(InventoryDbContext context)
        { _context = context; }

        [HttpPost("transaction")]
        public async Task<IActionResult> RecordSale([FromBody] SalesTransactions transactions)
        {
            transactions.SaleDate = DateTime.Now;
            _context.SalesTransactions.Add(transactions);

            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.StoreId == transactions.StoreId && i.ProductId == transactions.ProductId);

            if (inventory != null)
            {
                inventory.CurrentStock -= transactions.Quantity;
                inventory.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
