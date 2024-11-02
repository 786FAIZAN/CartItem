namespace CartItem.Models
{
    public class CouponActionLog
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int UserId { get; set; }
        public string ActionMessage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? FreeItemCartId { get; set; }
        public string FreeItemName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
