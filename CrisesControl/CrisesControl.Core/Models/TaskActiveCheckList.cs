using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskActiveCheckList
    {
        public int ActiveCheckListId { get; set; }
        public int CheckListId { get; set; }
        public int ActiveTaskId { get; set; }
        public string Description { get; set; } = null!;
        public int SortOrder { get; set; }
        public bool DoneOnly { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
