using Ardalis.SmartEnum;

namespace ShopRite.Core.Enumerations
{
    public sealed class ProductBrand : SmartEnum<ProductBrand, string>
    {
        public static readonly ProductBrand Nike = new ProductBrand(nameof(Nike), nameof(Nike));
        public static readonly ProductBrand Addidas = new ProductBrand(nameof(Addidas), nameof(Addidas));
        public static readonly ProductBrand Puma = new ProductBrand(nameof(Puma), nameof(Puma));


        public ProductBrand(string name, string value) : base(name, value)
        {
        }
    }
}
