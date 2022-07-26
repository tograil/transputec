using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Models
{
    public partial class IncidentTaskNote
    {
        public int IncidentTaskNotesId { get; set; }
        public int ObjectId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string? NoteType { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        
    }
}
