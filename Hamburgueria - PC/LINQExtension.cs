using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hamburgueria
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static void Rearrange(this ObservableCollection<View.Sale> sales)
        {
            List<View.Sale> items = sales.OrderByDescending(x => x.Date).ToList();

            sales.Clear();
            foreach (View.Sale it in items)
                sales.Add(it);
        }
    }
}
