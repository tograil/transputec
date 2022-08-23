using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments
{
    public class PaymentResponse
    {
        public string ResponseCode { get; set; }
        public string TransactionID { get; set; }
        public string ResponseMessage { get; set; }
        public string AuthCode { get; set; }
        public string RawMessage { get; set; }
        public decimal Amount { get; set; }
    }
}
