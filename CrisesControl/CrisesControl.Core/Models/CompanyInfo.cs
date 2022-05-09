using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models {
    public class CompanyRequestInfo {
        public int CompanyId { get; set; }
    }

    public class CompanyInfoReturn {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Company_Name { get; set; }
        public string CompanyLogo { get; set; }
        public string ContactLogo { get; set; }
        public string Website { get; set; }
        public string Master_Action_Plan { get; set; }
        public string Primary_Email { get; set; }
        public string PhoneISDCode { get; set; }
        public string SwitchBoardPhone { get; set; }
        public string Fax { get; set; }
        public string TimeZone { get; set; }
        public int ErrorId { get; set; }
        public string Message { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string CompanyProfile { get; set; }
        public DateTimeOffset AnniversaryDate { get; set; }
        public bool OnTrial { get; set; }
        public string CustomerId { get; set; }
        public string InvitationCode { get; set; }
    }
}
