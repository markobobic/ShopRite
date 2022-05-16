using Ardalis.SmartEnum;

namespace ShopRite.Core.Enumerations
{
    public sealed class OrderStatus : SmartEnum<OrderStatus, string>
    {
        public static readonly OrderStatus Pending = new OrderStatus(nameof(Pending), nameof(Pending));
        public static readonly OrderStatus PaymentRecived = new OrderStatus(nameof(PaymentRecived), nameof(PaymentRecived));
        public static readonly OrderStatus PaymentFailed = new OrderStatus(nameof(PaymentFailed), nameof(PaymentFailed));
        public OrderStatus(string name, string value) : base(name, value)
        {
        }
    }
}
