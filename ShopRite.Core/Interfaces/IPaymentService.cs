using ShopRite.Domain;
using System.Threading.Tasks;

namespace ShopRite.Core.Interfaces
{
    public interface IPaymentService
    {
        Task CreateOrUpdatePaymentIntent(CustomerBasket basket, string postCompanyId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}
