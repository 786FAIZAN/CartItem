namespace CartItem.Models
{
    public class CartItemViewModel
    {
        public IEnumerable<Item> Items { get; set; } // List of available items
        public List<CartItems> CartItems { get; set; } // List of items in the cart
    }
}
