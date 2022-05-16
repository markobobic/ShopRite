namespace ShopRite.Domain
{
    public record PostComany : BaseEntity
    {
        public string Name { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public string FullAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int Rating { get; set; }
    }
}
