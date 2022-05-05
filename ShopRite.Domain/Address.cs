using System.Text.Json.Serialization;

namespace ShopRite.Domain
{
    public record Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
        
        private string _fullAddress;
        [JsonIgnore]
        public string FullAddress
        {
            get { return _fullAddress; }
            private set { _fullAddress = $"{Street} {City} {ZipCode}"; }
        }

    }
}
