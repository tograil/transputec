using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompanyMessageTransactionStats
    {
        public int CompanyId { get; set; }
        [NotMapped]
        public Nullable<long> TotalPush { get; set; }
        [NotMapped]
        public Nullable<long> TotalPhone { get; set; }
        [NotMapped]
        public Nullable<long> TotalText { get; set; }
        [NotMapped]
        public Nullable<long> TotalEmail { get; set; }
    }
}
