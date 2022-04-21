using ShopRite.Domain;
using System;
using System.Collections.Generic;

namespace ShopRite.Core.Interfaces
{
    public interface ISessionService
    {
        string GetId();
        void AddProduct(Cart cartProduct);
        void RemoveProduct(int stockId, int qty);
        IEnumerable<TResult> GetCart<TResult>(Func<Cart, TResult> selector);
        void ClearCart();
    }
}
