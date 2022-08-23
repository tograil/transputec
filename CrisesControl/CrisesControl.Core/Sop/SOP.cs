using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class SOP
    {
        [NotMapped]
        public Sopheader Header { get; set; }
        public int IncidentID { get; set; }
        public int MyProperty { get; set; }
        public string Name { get; set; }
        public int SOPDocID { get; set; }
        public string IncidentTypeName { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public UserFullName SOPOwnerName { get; set; }
    }
}
