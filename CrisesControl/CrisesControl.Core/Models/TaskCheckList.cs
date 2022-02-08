using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskCheckList
    {
        public int CheckListId { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; } = null!;
        public int OptionCount { get; set; }
        public int SortOrder { get; set; }
        public bool DoneOnly { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
