﻿namespace SmartInventory.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
