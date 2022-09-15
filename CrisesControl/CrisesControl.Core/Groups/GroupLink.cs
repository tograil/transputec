using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Groups
{
    public class GroupLink
    {
        [NotMapped]
        public int LinkID { get; set; }
        [NotMapped]
        public string LinkName { get; set; }
    }
}
