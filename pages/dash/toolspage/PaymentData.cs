using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plz_fix.pages.dash.toolspage
{
    public class PaymentRecord
    {
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }

    public static class PaymentData
    {
        public static List<PaymentRecord> GetPayments()
        {
            return new List<PaymentRecord>
            {
                new PaymentRecord { Description = "School Fees - Term 1", Amount = "200 TND", Date = "Jan 15, 2024", Status = "Paid", StatusColor = "Green" },
                new PaymentRecord { Description = "School Fees - Term 2", Amount = "200 TND", Date = "Apr 15, 2024", Status = "Pending", StatusColor = "Red" }
            };
        }

        public static int GetTotalPayments() => GetPayments().Count;
        public static int GetMissingPayments() => GetPayments().Count(p => p.Status != "Paid");
    }
}
