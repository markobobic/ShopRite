namespace ShopRite.Domain
{
    public record OrderStatistics : BaseEntity
    {
        public decimal TotalIncome { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int TotalOrders { get; set; }
    }
}
