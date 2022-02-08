using System;

namespace CrisesControl.Core.Models
{
    public partial class EmailTemplate
    {
        public int TemplateId { get; set; }
        public string? Type { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? HtmlData { get; set; }
        public string? Locale { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int CompanyId { get; set; }
        public string? EmailSubject { get; set; }
        public int Status { get; set; }
    }
}
