using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskActiveCheckListResponse
    {
        public int ActiveCheckListResponseId { get; set; }
        public int ActiveCheckListId { get; set; }
        public int ResponseId { get; set; }
        public string Response { get; set; } = null!;
        public bool MarkDone { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
