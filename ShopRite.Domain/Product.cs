namespace ShopRite.Domain
{
    public record Product : BaseEntity
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
}
