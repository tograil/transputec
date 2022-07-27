using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class SectionGroup
    {
        public int SOPGroupID { get; set; }
        [NotMapped]
        public UserFullName OwnerName { get; set; }
    }
}
