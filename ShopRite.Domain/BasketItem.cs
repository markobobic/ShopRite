using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record BasketItem
    {
        public string Id { get; set; }
        public string Name { get; init; }
        public decimal Price { get; set; }
        public string ProductBrand { get; set; }
        public string ProductType { get; set; }
        public List<Stock> Sizes { get; set; }
        
    }
}