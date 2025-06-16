using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;

namespace SmartInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlgorithmController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public AlgorithmController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet("reorder-recommendations/{storeId}")]
        public async Task<IActionResult> GetReorderRecommendations(int storeId)
        {
            var today = DateTime.Today;
            var last30Days = today.AddDays(-30);

            // Get sales in the last 30 days
            var recentSales = await _context.SalesTransactions
                .Where(s => s.StoreId == storeId && s.SaleDate >= last30Days)
                .ToListAsync();

            var salesByProduct = recentSales
                .GroupBy(s => s.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var inventories = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId)
                .ToListAsync();

            var suggestions = new List<object>();

            foreach (var inv in inventories)
            {
                salesByProduct.TryGetValue(inv.ProductId, out var sales);

                int totalQty = sales?.Sum(s => s.Quantity) ?? 0;
                double avgDailySales = totalQty / 30.0;

                int weekdays = sales?.Count(s => s.SaleDate.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday) ?? 0;
                int weekends = sales?.Count(s => s.SaleDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) ?? 0;

                double seasonalityFactor = ((weekdays * 0.8) + (weekends * 1.4)) / 7.0;
                double adjustedSales = avgDailySales * seasonalityFactor;

                double safetyStock = adjustedSales * 2;
                double reorderPoint = adjustedSales * inv.Product.LeadTimeDays + safetyStock;

                int reorderQty = (int)Math.Ceiling(reorderPoint - inv.CurrentStock);
                reorderQty = (int)Math.Ceiling(reorderQty / 10.0) * 10; // round to nearest 10

                if (reorderQty > 0 && reorderQty <= inv.Product.MaxStorageQty)
                {
                    suggestions.Add(new
                    {
                        Product = inv.Product.Name,
                        SKU = inv.Product.SKU,
                        CurrentStock = inv.CurrentStock,
                        ReorderQty = reorderQty
                    });
                }
            }

            return Ok(suggestions);
        }

        [HttpGet("abc-analysis/{storeId}")]
        public async Task<IActionResult> GetABCAnalysis(int storeId)
        {
            var sales = await _context.SalesTransactions
                .Where(s => s.StoreId == storeId)
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            var totalRevenue = sales.Sum(x => x.Revenue);
            decimal runningTotal = 0;

            var result = sales.Select(s =>
            {
                runningTotal += s.Revenue;
                decimal percentage = (runningTotal / totalRevenue) * 100;
                string category = percentage <= 80 ? "A" :
                                  percentage <= 95 ? "B" : "C";
                var product = _context.Products.First(p => p.Id == s.ProductId);

                return new
                {
                    Name = product.Name,
                    Revenue = s.Revenue,
                    Category = category
                };
            });

            return Ok(result);
        }

        [HttpGet("inventory/{storeId}")]
        public async Task<IActionResult> GetInventory(int storeId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId)
                .Select(i => new
                {
                    Product = i.Product.Name,
                    SKU = i.Product.SKU,
                    CurrentStock = i.CurrentStock,
                    LastUpdated = i.LastUpdated
                })
                .ToListAsync();

            return Ok(inventory);
        }


    }
}
