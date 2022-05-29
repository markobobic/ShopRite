using Ardalis.SmartEnum;

namespace ShopRite.Core.Enumerations
{
    internal class PaymentMethod : SmartEnum<PaymentMethod, string>
    {
        public static readonly PaymentMethod OnDelivery = new PaymentMethod(nameof(OnDelivery), nameof(OnDelivery));
        public static readonly PaymentMethod Online = new PaymentMethod(nameof(Online), nameof(Online));
        public PaymentMethod(string name, string value) : base(name, value)
        {
        }
    }
}
