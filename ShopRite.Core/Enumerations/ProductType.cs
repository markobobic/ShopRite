using Ardalis.SmartEnum;

namespace ShopRite.Core.Enumerations
{
    public sealed class ProductType : SmartEnum<ProductType, string>
    {
        public static readonly ProductType Shoes = new ProductType(nameof(Shoes), nameof(Shoes));
        public static readonly ProductType Shirt = new ProductType(nameof(Shirt), nameof(Shirt));
        public ProductType(string name, string value) : base(name, value)
        {
        }
    }
}
