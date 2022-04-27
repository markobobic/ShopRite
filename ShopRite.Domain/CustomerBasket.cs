using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record CustomerBasket : BaseEntity
    {
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
