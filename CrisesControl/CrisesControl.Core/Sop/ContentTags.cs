using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class ContentTags
    {
        public int TagId { get; set; }
        public int TagCategoryId { get; set; }
        public string? TagName { get; set; }
        public string? SearchTerms { get; set; }
        public string? TagCategoryName { get; set; }
        public string? TagCategorySearchTerms { get; set; }
    }
}
