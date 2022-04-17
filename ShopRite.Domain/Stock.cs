using NUlid;

namespace ShopRite.Domain
{
    public record Stock 
    {
        public string Id { get; init; } = Ulid.NewUlid().ToString();
        public string Description { get; init; }
        public int Quantity { get; init; }
    }
}
