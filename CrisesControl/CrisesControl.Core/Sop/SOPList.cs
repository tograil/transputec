using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class SOPList
    {
        public Sopheader Header { get; set; }
        public int IncidentID { get; set; }
        public string Name { get; set; }
        public int SOPDocID { get; set; }
        public string IncidentTypeName { get; set; }
        public int Status { get; set; }
        public UserFullName Author { get; set; }
    }
}
