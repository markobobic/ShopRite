using NUlid;

namespace ShopRite.Domain
{
    public abstract record BaseEntity
    {
        public string Id { get; init; } = Ulid.NewUlid().ToString();
    }
}
