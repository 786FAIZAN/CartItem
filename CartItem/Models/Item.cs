namespace CartItem.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvlQuantity { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
