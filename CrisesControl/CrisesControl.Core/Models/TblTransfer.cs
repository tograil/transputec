using System;

namespace CrisesControl.Core.Models
{
    public partial class TblTransfer
    {
        public int Id { get; set; }
        public int? SourceRefId { get; set; }
        public int? TargetRefId { get; set; }
        public string? RefObject { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
