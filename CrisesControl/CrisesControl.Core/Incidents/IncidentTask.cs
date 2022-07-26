using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class IncidentTask
    {
        public int IncidentTaskNotesId { get; set; }
        public string Notes { get; set; }
        public string NoteType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<Attachment> Attachments { get; set; }

    }
}
