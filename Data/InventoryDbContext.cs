using Microsoft.EntityFrameworkCore;
using SmartInventory.Models;

namespace SmartInventory.Data
{
    public class InventoryDbContext:DbContext
    {
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<SalesTransactions> SalesTransactions { get; set; }
        public DbSet<ReorderRecommendation> ReorderRecommendations { get; set; }
        public DbSet<User> Users { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
