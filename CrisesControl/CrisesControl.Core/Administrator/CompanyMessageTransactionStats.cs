using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompanyMessageTransactionStats
    {
        public int CompanyId { get; set; }
        public Nullable<long> TotalPush { get; set; }
        public Nullable<long> TotalPhone { get; set; }
        public Nullable<long> TotalText { get; set; }
        public Nullable<long> TotalEmail { get; set; }
    }
}
