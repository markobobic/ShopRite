using Ardalis.GuardClauses;
using System;

namespace ShopRite.Core.Extensions
{
    public static class GuardExtensions
    {
        public static void False(this IGuardClause guardClause, bool condition, string errMsg = "")
        {
            if(condition is false)
                throw new Exception(errMsg);
        }
    }
}
