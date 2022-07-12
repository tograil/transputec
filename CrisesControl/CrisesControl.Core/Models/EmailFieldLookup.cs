using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Models
{
    public partial class EmailFieldLookup
    {
        public int FieldId { get; set; }
        public string? FieldName { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldDescription { get; set; }
        public int FieldType { get; set; }
        public string? ValidateField { get; set; }
        public string? SampleValue { get; set; }
       // public EmailTmplFieldMapping EmailTmplFieldMapping { get; set; }
    }
}
