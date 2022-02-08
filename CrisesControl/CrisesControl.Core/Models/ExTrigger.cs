using System;

namespace CrisesControl.Core.Models
{
    public partial class ExTrigger
    {
        public int ExTriggerId { get; set; }
        public int CompanyId { get; set; }
        public string? JobName { get; set; }
        public string? JobDescription { get; set; }
        public string? CommandLine { get; set; }
        public string? CommandLineParams { get; set; }
        public string? ActionType { get; set; }
        public bool IsEnabled { get; set; }
        public string? FailureReportEmail { get; set; }
        public string? SourceEmail { get; set; }
        public string JobKey { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string? SourceNumber { get; set; }
        public string? SourceNumberIsd { get; set; }
        public string? ColumnMappingFilePath { get; set; }
        public string? ColumnMappingFileType { get; set; }
        public string? ImportFileType { get; set; }
        public bool FileHasHeader { get; set; }
        public string? Delimiter { get; set; }
        public bool SendInvite { get; set; }
        public string? Smskey { get; set; }
        public bool AutoForceVerify { get; set; }
        public int JobIncidentId { get; set; }
        public string? JobType { get; set; }
        public string? SourceFilePath { get; set; }
        public string? ImportSource { get; set; }
        public bool DeleteSourceFile { get; set; }
    }
}
