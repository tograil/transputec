using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class GetSopResponse
    {
        public List<SOPList> SOPLists { get; set; }
        public SOP SOP { get; set; }
    }
}
