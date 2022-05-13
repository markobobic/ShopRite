using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record OrderItem
    {
        public string ProductId { get; init; }
        public List<Stock> Sizes { get; set; }
    }
}
