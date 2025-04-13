namespace w8d1_AdvancedUnitTesting.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public User User { get; set; } // Navigation property
    }
}
