using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRite.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV)) =>
               dict.TryGetValue(key, out TV value) ? value : defaultValue;
    }
}
