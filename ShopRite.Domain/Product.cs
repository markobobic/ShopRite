using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShopRite.Domain
{
    public record Product : BaseEntity
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string ProductBrand { get; set; }
        public string ProductType { get; set; }
        [JsonIgnore]
        public List<Stock> Stocks { get; set; } = new List<Stock>();
    }


}
