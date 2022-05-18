using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Core.Constants;
using ShopRite.Core.DTOs;
using ShopRite.Core.Extensions;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            private readonly UserManager<AppUser> _userManager;
            private readonly IHttpContextAccessor _context;

            public Handler(IConnectionMultiplexer redis,
                           IAsyncDocumentSession db,
                           IEmailService emailService,
                           UserManager<AppUser> userManager,
                           IHttpContextAccessor context)
            {
                _redis = redis.GetDatabase();
                _db = db;
                _emailService = emailService;
                _userManager = userManager;
                _context = context;
            }
            public async Task<CreateOrderResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await GetCurrentUser();
                var basket = await GetCurrentCustomerBasket(request);
               
                var response = new CreateOrderResponse();
                await Parallel.ForEachAsync(basket.Items, async (orderItem, cancellationToken) =>
                {
                    var product = await _db.LoadAsync<Product>(orderItem.Id);
                    var stocksDict = product.Stocks.ToDictionary(key => key.Size, value => value.Quantity);
                    var isOutOfStock = TotalSumFromBasket(basket) > CurrentStockInDatabase(stocksDict);
                    orderItem.Sizes.ForEachParallel(requestedSize => SubstractFromStock(ref response, product, stocksDict, isOutOfStock, requestedSize));
                });
                
                if (response.SuccessfulOrders[false].Any())
                {
                    var outOfStockTask = _emailService.SendEmailOutOfStock(new OrderDTO(response.SuccessfulOrders[false], basket.TotalPrice));
                    await Task.WhenAll(outOfStockTask, _db.SaveChangesAsync());
                    return response;
                }

                
                var order = new Domain.Order()
                {
                    OrderItems = basket.Items.Select(x => new OrderItem { ProductId = x.Id, Sizes = x.Sizes }).ToList(),
                    BuyerEmail = request.CreateOrderRequest.BuyerEmail,
                    TotalPrice = basket.TotalPrice,
                    Month = Date.Months[DateTime.Today.Month],
                    Year = DateTime.Today.Year,
                    ShipToAddress = currentUser?.Address ?? new Address(),
                };
                 var emailSuccessfulOrder = _emailService.SendEmailSuccessfulOrder(new OrderDTO(response.SuccessfulOrders[true], basket.TotalPrice),
                                                                 request.CreateOrderRequest.BuyerEmail);
                 
                 await Task.WhenAll(emailSuccessfulOrder, _db.StoreAsync(order), _db.SaveChangesAsync());
                  

                return response;
            }

            private async Task<CustomerBasket> GetCurrentCustomerBasket(Command request)
            {
                var data = await _redis.StringGetAsync(request.CreateOrderRequest.BasketId);
                var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
                return basket;
            }

            private async Task<AppUser> GetCurrentUser()
            {
                AppUser currentUser = null;
                var email = _context.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(email) is false)
                {
                    currentUser = await _userManager.FindByEmailAsync(email);
                }

                return currentUser;
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
            public ConcurrentBag<string> ProductsOutOfStack { get; set; } = new();
            public IDictionary<bool, ConcurrentBag<ProductDetailDTO>> SuccessfulOrders { get; set; }
                      = new ConcurrentDictionary<bool, ConcurrentBag<ProductDetailDTO>>
                      {
                          [true] = new ConcurrentBag<ProductDetailDTO>(),
                          [false] = new ConcurrentBag<ProductDetailDTO>(),
                      };
            public decimal TotalPrice { get; set; }
        }

    }
}
