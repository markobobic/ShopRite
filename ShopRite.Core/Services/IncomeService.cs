using Coravel.Invocable;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using ShopRite.Core.Constants;
using ShopRite.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Core.Services
{
    public class IncomeService : IInvocable, ICancellableInvocable
    {
        private readonly IAsyncDocumentSession _db;

        public IncomeService(IAsyncDocumentSession db)
        {
            _db = db;
        }
        public CancellationToken CancellationToken { get; set; }

        public async Task Invoke()
        {
            var previousMonth = Date.Months[DateTime.Today.AddMonths(-1).Month];
            var orders = await _db.Query<Order>().Where(x => x.Month == previousMonth && x.Year == DateTime.Now.Year).ToListAsync();

            var orderStatistic = new OrderStatistics
            {
                Month = previousMonth,
                Year = DateTime.Now.Year,
                TotalIncome = orders?.Sum(x => x.TotalPrice) ?? 0,
            };

           await _db.StoreAsync(orderStatistic);
           await _db.SaveChangesAsync();
        }
    }
}
