using Microsoft.AspNetCore.Identity;
using SmartInventory.Models;

namespace SmartInventory.Data
{
    public static class SeedData
    {
        public static void Initialize(InventoryDbContext context)
        {
            if (context.Stores.Any()) return;

            var stores = new List<Store>
            {
                new Store {Name="7-Eleven Colpetty", Code="CMB01", Address="Colpetty"},
                new Store {Name="7-Eleven Fort", Code="CMB02", Address="Fort"},
                new Store {Name="7-Eleven Kandy", Code="KDY01", Address="Kandy"}
            };
            context.Stores.AddRange(stores);
            context.SaveChanges();

            var products = new List<Product>();
            for (int i = 0; i < 30; i++)
            {
                products.Add(new Product
                {
                    SKU = $"SKU{i:000}",
                    Name = $"Product{i}",
                    Category = i % 2 == 0 ? "Food" : "Beverage",
                    Price = 100 + i * 10,
                    MinStockLevel = 20,
                    LeadTimeDays = 3,
                    MaxStorageQty = 500
                });
            }
            context.Products.AddRange(products);
            context.SaveChanges();

            var inventory = new List<Inventory>();
            foreach (var item in stores)
            {
                foreach (var productitem in products)
                {
                    inventory.Add(new Inventory
                    {
                        StoreId = item.Id,
                        ProductId = productitem.Id,
                        CurrentStock = 100,
                        LastUpdated = DateTime.Now,
                    });
                }
            }

            context.Inventories.AddRange(inventory);
            context.SaveChanges();

            var users = new User
            {
                    UserName="admin",
                    Email="admin@gmail.com",
                    Role="StoreManager",
                    StoreId = stores.First().Id                    
            };
            users.PasswordHash = new PasswordHasher<User>().HashPassword(users, "admin123");
            context.Users.AddRange(users);
            context.SaveChanges();
        }

    }
}
