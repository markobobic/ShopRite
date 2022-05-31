using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRite.Core.Services
{
    public class GoogleService : IGoogleService
    {
        public DistanceType DetermineDistanceType(Address source, Address destination)
        {
            var result = "TODO"; //TODO call api
            switch (result)
            {
                case "0": return DistanceType.SmallDistance;
                case "1": return DistanceType.SmallDistance;
                case "2": return DistanceType.SmallDistance;
                default:
                    break;
            }
            return default(DistanceType);
        }
    }
}
