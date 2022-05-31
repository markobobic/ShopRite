using ShopRite.Domain;

namespace ShopRite.Core.Interfaces
{
    public interface IGoogleService
    {
        DistanceType DetermineDistanceType(Address source, Address destination);
    }
}
