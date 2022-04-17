using System.Collections.Generic;

namespace ShopRite.Domain
{
    public record Product : BaseEntity
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; set; }
        public List<Stock> Stocks { get; set; } = new List<Stock>();
    }


}
