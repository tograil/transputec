using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class PublicAlertUserList
    {
        public int MessageId { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
        public string? Postcode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
