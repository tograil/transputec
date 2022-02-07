using System;

namespace CrisesControl.Core.Models
{
    public partial class ImportDumpHeader
    {
        public int ImportDumpHeaderId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; } = null!;
        public string? MappingFileName { get; set; }
        public string? FileName { get; set; }
        public string? Status { get; set; }
        public bool? SendInvite { get; set; }
        public bool AutoForceVerify { get; set; }
        public int ImportTriggerId { get; set; }
        public string? JobType { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
