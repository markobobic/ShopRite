using NUlid;

namespace ShopRite.Domain
{
    public record BasketItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}