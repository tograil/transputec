namespace CrisesControl.Core.Models
{
    public partial class ImportTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = null!;
        public string TemplateFile { get; set; } = null!;
        public string TemplateType { get; set; } = null!;
    }
}
