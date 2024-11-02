namespace CartItem.Models
{
    public class CartItems
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } // New property for item name
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedDate { get; set; }
    }

}
