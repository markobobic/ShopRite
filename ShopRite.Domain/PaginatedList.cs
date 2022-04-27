using System;
using System.Collections.Generic;

namespace ShopRite.Domain
{
    public class PaginatedList<T>
    {
        public List<T> Data { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(TotalItems / (double)PageSize);
            }
        }
    }
}
