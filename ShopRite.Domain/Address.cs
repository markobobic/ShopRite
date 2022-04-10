namespace ShopRite.Domain
{
    public record Address
    {
        public string Address1 { get; init; }
        public string Address2 { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
    }
}
