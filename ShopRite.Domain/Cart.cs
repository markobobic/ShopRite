namespace ShopRite.Domain
{
    public record Cart
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
