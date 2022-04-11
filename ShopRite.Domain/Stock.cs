namespace ShopRite.Domain
{
    public record Stock : BaseEntity
    {
        public string Description { get; init; }
        public int Quantity { get; init; }
        public string ProductId { get; init; }
    }
}
