using System;

namespace CrisesControl.Core.Models
{
    public partial class ModelMappingLog
    {
        public int ModelMappingLogId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string? MethodName { get; set; }
        public string? ControllerName { get; set; }
        public string? InputData { get; set; }
        public DateTimeOffset EntryDate { get; set; }
    }
}
