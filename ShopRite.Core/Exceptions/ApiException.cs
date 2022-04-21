using ShopRite.Core.Responses;

namespace ShopRite.Core.Exceptions
{
    public record ApiException : ApiResponse
    {
        public ApiException(int statusCode, string message = null, string details = null) : base(statusCode, message)
        {
            Details = details;
        }

        public string Details { get; init; }
    }
}
