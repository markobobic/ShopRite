using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record Order : BaseEntity
    {
        public string OrderRef { get; init; }
        public Address Address { get; init; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
