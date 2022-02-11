using System;

namespace CrisesControl.Core.Models
{
    public partial class ExceptionLog
    {
        public int ExceptionLogId { get; set; }
        public string? ErrorId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ControllerName { get; set; }
        public string? MethodName { get; set; }
        public int? CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset EntryDate { get; set; }
    }
}
