using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class UserParams
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public string ParamType { get; set; }
        public string Module { get; set; }
        public int ParamOrder { get; set; }
    }
}
