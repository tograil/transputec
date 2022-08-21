using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class CompanyUserCountReturn
    {
        public int UserId { get; set; }
        public long TotalPushReceived { get; set; }
        public long TotalPhoneReceived { get; set; }
        public long TotalTextReceived { get; set; }
        public long TotalEmailReceived { get; set; }
    }
}
