using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class FailedAttempts
    {
       
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset MessageStartTime { get; set; }
    }
}
