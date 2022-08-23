using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class GetSopResponse
    {
        public List<SOPList> SOPLists { get; set; }
        [NotMapped]
        public SOP SOP { get; set; }
    }
}
