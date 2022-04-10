namespace ShopRite.Domain
{
    public record Stock : BaseEntity
    {
        public string Description { get; init; }
        public string Quantity { get; init; }
        public Product Product { get; init; }
    }
}
