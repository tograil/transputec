using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System
{
    public class ModelLogReturn
    {
        public DateTimeOffset EntryDate { get; set; }
        public string ControllerName { get; set; }
        public string MethodName { get; set; }
        public string InputData { get; set; }
    }
}
