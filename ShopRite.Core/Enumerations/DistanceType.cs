using Ardalis.SmartEnum;

namespace ShopRite.Core.Enumerations
{
    public class DistanceType : SmartEnum<DistanceType, string>
    {
        public static readonly DistanceType SmallDistance = new DistanceType(nameof(SmallDistance), nameof(SmallDistance));
        public static readonly DistanceType MediumDistance = new DistanceType(nameof(MediumDistance), nameof(MediumDistance));
        public static readonly DistanceType BigDistance = new DistanceType(nameof(BigDistance), nameof(BigDistance));

        public DistanceType(string name, string value) : base(name, value)
        {
        }
    }
}
