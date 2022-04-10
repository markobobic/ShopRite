namespace ShopRite.Platform.Products
{
    public class UpdateProduct
    {
        public class Command
        {
            public Command(ProductRequest request)
            {
                Request = request;
            }

            public ProductRequest Request { get; set; }
        }

        public class ProductRequest
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
        }
    }
}
