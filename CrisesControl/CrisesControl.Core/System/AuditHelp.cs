using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System
{
    public class AuditHelp
    {
        [NotMapped]
        public UserFullName UserName { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string EventStatement { get; set; }
    }
}
