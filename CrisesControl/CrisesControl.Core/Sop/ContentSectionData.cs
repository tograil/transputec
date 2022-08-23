using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class ContentSectionData
    {
        public DateTimeOffset UpdatedOn { get; set; }
        public int SOPHeaderID { get; set; }
        public int ContentSectionID { get; set; }
        public int ContentID { get; set; }
        public string SectionType { get; set; }
        public string SectionDescription { get; set; }
        public string SectionName { get; set; }
        public int SectionOrder { get; set; }
        public List<SectionGroup> SectionGroups { get; set; }
        public List<SOPContentTag> SOPContentTags { get; set; }

    }
}
