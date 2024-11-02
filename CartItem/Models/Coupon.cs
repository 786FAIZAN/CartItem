namespace CartItem.Models
{
    public class Coupon
    {
        public int Id { get; set; }              // Unique identifier for the coupon
        public string Code { get; set; }         // Coupon code (e.g., "DISCOUNT10")
        public DateTime Validity { get; set; }   // Expiration date of the coupon
        public bool IsUsed { get; set; }     // Indicates if the coupon has been used
        public int UserId { get; set; }
    }
}
