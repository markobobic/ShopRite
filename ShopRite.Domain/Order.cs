using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record Order : BaseEntity
    {
        public string BuyerEmail { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
