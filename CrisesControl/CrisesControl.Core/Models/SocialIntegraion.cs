using System;

namespace CrisesControl.Core.Models
{
    public partial class SocialIntegraion
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? AccountName { get; set; }
        public string AccountType { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
        public string AuthSecret { get; set; } = null!;
        public string AdnlKeyOne { get; set; } = null!;
        public string AdnlKeyTwo { get; set; } = null!;
        public DateTimeOffset? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
