using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ShopRite.Domain
{
    
    public enum DistanceType
    {
        
        SmallDistance,
        MediumDistance,
        BigDistance
    }
    public record PostCompany : BaseEntity
    {
        public string Name { get; set; }
        public Dictionary<DistanceType, DeliveryMethod> DeliveryMethods { get; set; }
        public string FullAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int Rating { get; set; }
    }
}
