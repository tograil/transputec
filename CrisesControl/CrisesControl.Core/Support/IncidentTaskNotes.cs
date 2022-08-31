using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class IncidentTaskNotes
    {
        public int IncidentTaskNotesId { get; set; }
        public int ObjectId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string NoteType { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
