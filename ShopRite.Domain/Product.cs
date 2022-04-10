namespace ShopRite.Domain
{
    public record Product : BaseEntity
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; set; }
    }
}
