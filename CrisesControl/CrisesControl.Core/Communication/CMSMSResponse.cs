using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication
{
    public class CMSMSResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public List<CMResult> Result { get; set; }
    }
}
