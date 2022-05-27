using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class GetIncidentDataByActivationRefKeyContactsResponse
    {
        public int UserId { get; set; }
        public string KeyContactFirstName { get; set; }
        public string KeyContactLastName { get; set; }
        public string KeyContactImage { get; set; }
        public string KeyContactEmail { get; set; }
        public string KeyContactISDCode { get; set; }
        public string KeyContactMobileNo { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
