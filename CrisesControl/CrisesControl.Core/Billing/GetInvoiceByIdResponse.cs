using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class GetInvoiceByIdResponse: BillingDetailsReturn
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public new string UserName
        {
            get
            {
                return $"{UserFirstName ?? string.Empty} {UserLastName ?? string.Empty}";
            }
        }
    }
}
