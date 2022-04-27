using System.Collections.Generic;

namespace ShopRite.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV)) =>
               dict.TryGetValue(key, out TV value) ? value : defaultValue;
        
    }
}
