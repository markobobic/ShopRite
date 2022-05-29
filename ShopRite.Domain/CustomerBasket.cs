using System.Collections.Generic;
using System.Linq;

namespace ShopRite.Domain
{
    public record CustomerBasket : BaseEntity
    {
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        private long _totalPrice;
        public long TotalPrice
        {
            get { return _totalPrice = (long)Items.Sum(x => x.Sizes.Sum(x => x.Quantity) * (x.Price * 100)); }
            private set { _totalPrice = (long)Items.Sum(x => x.Sizes.Sum(x => x.Quantity) * (x.Price * 100)); }
        }

    }
}
