using System;

namespace CrisesControl.Core.Models
{
    public partial class AppLanguage
    {
        public int LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public string? Locale { get; set; }
        public string? IconFolder { get; set; }
        public string? IconUrl { get; set; }
        public string? FlagIcon { get; set; }
        public int? Status { get; set; }
        public string? Platform { get; set; }
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
