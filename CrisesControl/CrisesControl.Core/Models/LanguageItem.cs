using System;

namespace CrisesControl.Core.Models
{
    public partial class LanguageItem
    {
        public int LanguageItemId { get; set; }
        public string? LangKey { get; set; }
        public string? LangValue { get; set; }
        public string? Locale { get; set; }
        public string? ErrorCode { get; set; }
        public string? Title { get; set; }
        public string? Options { get; set; }
        public string? ObjectType { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string? Comment { get; set; }
        public string? LangFile { get; set; }
    }
}
