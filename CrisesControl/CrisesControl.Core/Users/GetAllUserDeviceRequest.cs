using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class GetAllUserDeviceRequest
    {
        public int OutUserCompanyId { get; set; }
        public int UserID { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string SearchString { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public string CompanyKey { get; set; }
    }
}
