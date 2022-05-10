using System.Collections.Generic;
using System.Linq;

namespace ShopRite.Domain
{
    public record CustomerBasket : BaseEntity
    {
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice = Items.Sum(x => x.Sizes.Sum(x => x.Quantity) * x.Price); }
            private set { _totalPrice = Items.Sum(x => x.Sizes.Sum(x => x.Quantity) * x.Price); }
        }

    }
}
