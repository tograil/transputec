using System;

namespace CrisesControl.Core.Models
{
    public partial class PublicAlertTemplate
    {
        public int TemplateId { get; set; }
        public int CompanyId { get; set; }
        public string TemplateName { get; set; } = null!;
        public int? EmailColIndex { get; set; }
        public int? PhoneColIndex { get; set; }
        public string? FileName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
