using Microsoft.AspNetCore.Mvc;

namespace SmartInventory.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
