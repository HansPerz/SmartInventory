using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;
using SmartInventory.Models;

namespace SmartInventory.Controllers
{
    [Authorize(AuthenticationSchemes ="CookieAuth")]
    public class HomeController : Controller
    {
        private readonly InventoryDbContext _context;

        public HomeController(InventoryDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;

            var user = await _context.Users
                .Include(u => u.Store)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return Unauthorized();
            }

            var storeId = user.StoreId;

            var lowStock = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId && i.CurrentStock <= i.Product.MinStockLevel)
                .ToListAsync();

            var reOrderSuggestions = lowStock.Select(i => new
            {
                Product = i.Product.Name,
                CurrentStock = i.CurrentStock,
                ReorderQty = i.Product.MinStockLevel * 2 - i.CurrentStock
            }).ToList();

            var sales = await _context.SalesTransactions
                .Where(s => s.StoreId == storeId)
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Revenue = g.Sum(s => s.Quantity * s.UnitPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            var totalRevenue = sales.Sum(s => s.Revenue);
            decimal runningTotal = 0;
            var abcResult = sales.Select(s =>
            {
                runningTotal += s.Revenue;
                var percentage = runningTotal / totalRevenue * 100;
                string category = percentage <= 80 ? "A" : percentage <= 95 ? "B" : "C";
                var product = _context.Products.First(p => p.Id == s.ProductId);
                return new { product.Name, s.Revenue, category = category };
            }).ToList();

            ViewBag.LowStock = lowStock;
            ViewBag.Reorders = reOrderSuggestions;
            ViewBag.ABC = abcResult;

            return View();
        }
    }
}
