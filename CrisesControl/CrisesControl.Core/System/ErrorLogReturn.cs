using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System
{
    public class ErrorLogReturn
    {
        public DateTimeOffset EntryDate { get; set; }
        public string ControllerName { get; set; }
        public string MethodName { get; set; }
        public string Exception { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
    }
}
