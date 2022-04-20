using ShopRite.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopRite.Core.Interfaces
{
    public interface IStockService
    {
        Task CreateStock(string productId, Stock stock);
        Task DeleteStock(string productId, string stockId);
        Task UpdateStockRange(string productId, List<Stock> stockList);

        Stock GetStockWithProduct(string productId, int stockId);
        Task<bool> EnoughStock(string productId, int stockId, int qty);
        Task PutStockOnHold(string productId, int stockId, int qty, string sessionId);

        Task RemoveStockFromHold(string productId, string sessionId);
        Task RemoveStockFromHold(string productId, int stockId, int qty, string sessionId);
        Task RetrieveExpiredStockOnHold();
    }
}
