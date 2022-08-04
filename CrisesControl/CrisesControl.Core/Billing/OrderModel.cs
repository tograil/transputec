using CrisesControl.Core.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class OrderModel : CcBase
    {
        OrderModel()
        {
            ContractType = "FIRST";
        }
        public int OrderId { get; set; }
        public int ContractDuration { get; set; }
        public decimal ContractValue { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public int KeyholderCount { get; set; }
        public int StaffCount { get; set; }
        public string OrderStatus { get; set; }
        public string TigerOrderNo { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public DateTime ContractStartDate { get; set; }
        public List<Product> Products { get; set; }
        public string ContractType { get; set; }
        public int Activated { get; set; }
        public decimal Discount { get; set; }
    }

    public class Product
    {
        public int ModuleID { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int Unit { get; set; }
        public string Added { get; set; }
        public decimal Discount { get; set; }
        public string ItemName { get; set; }
        public string ChargeType { get; set; }
    }
}
