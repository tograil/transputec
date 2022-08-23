using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication
{
    public class CMResult
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Standarderror { get; set; }
        public string Errordescription { get; set; }
        public int? Status { get; set; }
        public string Sstatusdescription { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Countryiso { get; set; }
        public string Countryname { get; set; }
        public string Mccmnc { get; set; }
        public string Operatorname { get; set; }
        public int Deliverytime { get; set; }
        public int Datacodingscheme { get; set; }
        public string Userdataheader { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public decimal? LocalPrice { get; set; }
        public string LocalCurrency { get; set; }
        public string Tariff { get; set; }
        public string Customgrouping { get; set; }
        public string Customgrouping2 { get; set; }
        public string Reference { get; set; }
        public string Direction { get; set; }
        [NotMapped]
        public CMMultiPart Multipart { get; set; }
    }
}
