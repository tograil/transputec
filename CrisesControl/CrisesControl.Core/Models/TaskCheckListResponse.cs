using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskCheckListResponse
    {
        public int CheckListResponseId { get; set; }
        public int CheckListId { get; set; }
        public int ResponseId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
