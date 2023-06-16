using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class TransactionItemDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeviceAddress { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public decimal Price { get; set; }
        public int NumSegments { get; set; }
        public int Duration { get; set; }
        public string Direction { get; set; }
        public decimal TransactionRate { get; set; }
        public decimal MinimumPrice { get; set; }
        public decimal VATRate { get; set; }
    }
}
