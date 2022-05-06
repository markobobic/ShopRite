using System.Text.Json.Serialization;

namespace ShopRite.Core.Configurations
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AWS
    {
        [JsonPropertyName("Profile")]
        public string Profile { get; set; }

        [JsonPropertyName("Region")]
        public string Region { get; set; }

        [JsonPropertyName("BucketName")]
        public string BucketName { get; set; }
    }

    public class FluentEmail
    {

        [JsonPropertyName("Host")]
        public string Host { get; set; }

        [JsonPropertyName("Port")]
        public int Port { get; set; }

        [JsonPropertyName("Username")]
        public string Username { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; }
    }

    public class Database
    {
        [JsonPropertyName("Urls")]
        public string[] Urls { get; set; }

        [JsonPropertyName("RavenDatabaseName")]
        public string RavenDatabaseName { get; set; }

        [JsonPropertyName("RedisDatabaseName")]
        public string RedisDatabaseName { get; set; }
    }

    public class Emails
    {
        [JsonPropertyName("RetailMail")]
        public string RetailMail { get; set; }
        [JsonPropertyName("CompanyMail")]
        public string CompanyMail { get; set; }
    }

    public class Logging
    {
        [JsonPropertyName("LogLevel")]
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        [JsonPropertyName("Default")]
        public string Default { get; set; }

        [JsonPropertyName("Microsoft")]
        public string Microsoft { get; set; }

        [JsonPropertyName("Microsoft.Hosting.Lifetime")]
        public string MicrosoftHostingLifetime { get; set; }
    }

    

    public class GlobalConfiguration
    {
        [JsonPropertyName("Logging")]
        public Logging Logging { get; set; }

        [JsonPropertyName("Database")]
        public Database Database { get; set; }

        [JsonPropertyName("AWS")]
        public AWS AWS { get; set; }

        [JsonPropertyName("Token")]
        public Token Token { get; set; }

        [JsonPropertyName("FluentEmail")]
        public FluentEmail FluentEmail { get; set; }

        [JsonPropertyName("Emails")]
        public Emails Emails { get; set; }
    }

    public class Token
    {
        [JsonPropertyName("Key")]
        public string Key { get; set; }

        [JsonPropertyName("Issuer")]
        public string Issuer { get; set; }
    }


}
