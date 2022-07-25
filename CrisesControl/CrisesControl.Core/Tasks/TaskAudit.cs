using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Tasks
{
    public class TaskAudit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FileName { get; set; }
        public int AttachmentID { get; set; }
        [NotMapped]
        public UserFullName ActionBy { get; set; }
        public int TaskActionTypeID { get; set; }
        public string ActionDescription { get; set; }
        public DateTimeOffset ActionDate { get; set; }
    }
}
