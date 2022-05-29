using System.Text.Json.Serialization;

namespace ShopRite.Core.Configurations
{
    public class StripeConfig
    {
        [JsonPropertyName("StripeSettings")]
        public StripeSettings StripeSettings { get; set; }
    }
    public class StripeSettings
    {
        [JsonPropertyName("PublishableKey")]
        public string PublishableKey { get; set; }
        [JsonPropertyName("SecretKey")]
        public string SecretKey { get; set; }
        [JsonPropertyName("WebhookSecret")]
        public string WebhookSecret { get; set; }
    }
}
