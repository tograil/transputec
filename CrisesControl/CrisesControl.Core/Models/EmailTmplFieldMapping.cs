namespace CrisesControl.Core.Models
{
    public partial class EmailTmplFieldMapping
    {
        public int FieldId { get; set; }
        public string TemplateCode { get; set; } = null!;
        public EmailFieldLookup EmailFieldLookup { get; set; }
    }
}
