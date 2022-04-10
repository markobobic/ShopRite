namespace ShopRite.Domain
{
    public record OrderItem
    {
        public string ProductId { get; init; }
        public Stock Stock { get; init; }
    }
}
