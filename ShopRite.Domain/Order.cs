using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record Order : BaseEntity
    {
        public string BuyerEmail { get; set; }
        public Address ShipToAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalPrice { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public PostCompany PostComany { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
