using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompanyPackageFeatureList
    {
        public int ParentID { get; set; }
        public int ModuleID { get; set; }
        public string ProductCode { get; set; }
        public int SecurityObjectID { get; set; }
        public string SecurityKey { get; set; }
        public string SecurityLabel { get; set; }
        public string ModuleName { get; set; }
        public int Status { get; set; }
        public decimal Rate { get; set; }
        public string TransactionTypeName { get; set; }
        public bool IsPaid { get; set; }
        public string ChargeType { get; set; }
        public string PaymentPeriod { get; set; }
        public decimal TransactionRate { get; set; }
        public int TransactionTypeID { get; set; }
        public string ModuleType { get; set; }
        public int Units { get; set; }
        //public decimal Amount { get; set; }
        public int LinkID { get; set; }
        public int ModuleParentID { get; set; }
        public decimal Discount { get; set; }
        public bool RequireKeyHolder { get; set; }
        public bool RequireAdmin { get; set; }
        public int ForIncidentManager { get; set; }
    }
}
