using MediatR;
using Raven.Client.Documents.Session;
using ShopRite.Core.DTOs;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Orders
{
    public class CreateOrder
    {
        public class Command : IRequest<CreateOrderResponse>
        {
            public CreateOrderRequest CreateOrderRequest { get; set; }
        }
        public class Handler : IRequestHandler<Command, CreateOrderResponse>
        {
            private readonly IDatabase _redis;
            private readonly IAsyncDocumentSession _db;
            private readonly IEmailService _emailService;

            public Handler(IConnectionMultiplexer redis,
                           IAsyncDocumentSession db,
                           IEmailService emailService)
            {
                _redis = redis.GetDatabase();
                _db = db;
                _emailService = emailService;
            }
            public async Task<CreateOrderResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = await _redis.StringGetAsync(request.CreateOrderRequest.BasketId);
                var response = new CreateOrderResponse();
                var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
                foreach (var orderItem in basket?.Items)
                {
                    var product = await _db.LoadAsync<Product>(orderItem.Id);
                    var stocksDict = product.Stocks.ToDictionary(key => key.Size, value => value.Quantity);
                    var isOutOfStock = TotalSumFromBasket(basket) > CurrentStockInDatabase(stocksDict);
                    foreach (var requestedSize in orderItem.Sizes)
                    {
                        SubstractFromStock(ref response, product, stocksDict, isOutOfStock, requestedSize);
                    }
                }
                if (response.SuccessfulOrders[false].Any())
                   
                    await _emailService.SendEmailOutOfStock(new OrderDTO(response.SuccessfulOrders[false], basket.TotalPrice));
                else
                    await _emailService.SendEmailSuccessfulOrder(new OrderDTO(response.SuccessfulOrders[true], basket.TotalPrice),
                                                                 request.CreateOrderRequest.BuyerEmail);


                await _db.SaveChangesAsync();

                return response;
            }

            private void SubstractFromStock(ref CreateOrderResponse response, Product product, Dictionary<string, int> stocksDict, bool isOutOfStock, Stock requestedSize)
            {
                if (stocksDict.ContainsKey(requestedSize.Size))
                {
                    var currentQuantity = product.Stocks
                                         .FirstOrDefault(x => x.Size == requestedSize.Size);
                    TrySubtractFromStock(product, requestedSize, currentQuantity, response, isOutOfStock);
                }
            }
            private bool TrySubtractFromStock(Product product, Stock requestedSize, Stock currentQuantity, CreateOrderResponse response, bool isOutOfStock)
            {
                if (currentQuantity.Quantity >= requestedSize.Quantity && !isOutOfStock)
                {
                    currentQuantity.Quantity -= requestedSize.Quantity;
                    response.SuccessfulOrders[true]
                        .Add(new ProductDetailDTO { Name = product.Name, Size = currentQuantity.Size, RequestedQuantity = requestedSize.Quantity, Price = product.Price });
                    return true;
                }
                if (requestedSize.Quantity > currentQuantity.Quantity && isOutOfStock)
                {
                    response.SuccessfulOrders[false]
                     .Add(new ProductDetailDTO { Name = product.Name, Size = currentQuantity.Size, Price = product.Price });
                    product.IsOutOfStock = true;
                    response.ProductsOutOfStack.Add($"Product {product.Name} is out of stack for size {requestedSize.Size}.");
                }

                return false;
            }
            private static int CurrentStockInDatabase(Dictionary<string, int> stocksDict) => stocksDict.Sum(x => x.Value);
            private static int TotalSumFromBasket(CustomerBasket basket) => basket.Items.Sum(x => x.Sizes.Sum(x => x.Quantity));


        }
        public class CreateOrderRequest
        {
            public string BuyerEmail { get; set; }
            public string BasketId { get; set; }
        }

        public class CreateOrderResponse
        {
            public List<string> ProductsOutOfStack { get; set; } = new();
            public Dictionary<bool, List<ProductDetailDTO>> SuccessfulOrders { get; set; }
                      = new()
                      {
                          { true, new List<ProductDetailDTO>() },
                          { false, new List<ProductDetailDTO>() },
                      };
            public decimal TotalPrice { get; set; }
        }

    }
}
