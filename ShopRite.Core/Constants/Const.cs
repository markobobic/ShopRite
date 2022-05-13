using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShopRite.Core.Constants
{
    public static class Assemblies
    {
        public const string ShopRitePlatform = "ShopRite.Platform";
        public const string ShopRiteCore = "ShopRite.Core";
    }
    public static class PaginationConfig
    {
        public const int DefaultLimit = 20;
        public const int FirstPageNumber = 1;
    }
    public static class UserRole
    {
        public const string Admin = "Admin";
        public const string Buyer = "Buyer";
    }
    public static class Date
    {
        public static readonly Dictionary<int, string> Months = Enumerable.Range(1, 12).Select(i => new KeyValuePair<int, string>(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i))).ToDictionary(x => x.Key, x => x.Value);
    }
}
