using System;

namespace CrisesControl.Core.Models
{
    public partial class Log4NetLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Thread { get; set; } = null!;
        public string? ControllerName { get; set; }
        public string? MethodName { get; set; }
        public int? CompanyId { get; set; }
        public string Level { get; set; } = null!;
        public string Logger { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Exception { get; set; }
    }
}
