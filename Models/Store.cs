namespace SmartInventory.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}
