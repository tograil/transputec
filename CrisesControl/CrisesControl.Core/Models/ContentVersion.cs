using System;

namespace CrisesControl.Core.Models
{
    public partial class ContentVersion
    {
        public int ContentVersionId { get; set; }
        public int PrimaryContentId { get; set; }
        public int LastContentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
