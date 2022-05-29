using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ShopRite.Core.DTOs
{
    public class OrderDTO
    {
        public ConcurrentBag<ProductDetailDTO> ProductDetails { get; set; }
        public decimal TotalPrice { get; init; }
        public OrderDTO(ConcurrentBag<ProductDetailDTO> productDetails, decimal totalPrice)
        {
            ProductDetails = productDetails;
            TotalPrice = totalPrice;
        }

    }
    public class ProductDetailDTO
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int RequestedQuantity { get; set; }
    }
}
