using System;
using System.Globalization;

namespace ShopRite.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);
    }
}
