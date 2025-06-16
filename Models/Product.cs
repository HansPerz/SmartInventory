namespace SmartInventory.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int MinStockLevel { get; set; }
        public int LeadTimeDays { get; set; }
        public int MaxStorageQty { get; set; }
    }
}
