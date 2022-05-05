using NUlid;
using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record BasketItem
    {
        public Product Product { get; set; }
        public List<Stock> Sizes { get; set; }
    }
}