using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class Section
    {
        public int SOPHeaderID { get; set; }
        public int ContentID { get; set; }
        public int ContentSectionID { get; set; }
        public string SectionType { get; set; }
        public string SectionDescription { get; set; }
        public int SectionOrder { get; set; }
        [NotMapped]
        public List<int> SOPContentTags { get; set; }
        [NotMapped]
        public List<int> SOPGroups { get; set; }
        public string SectionName { get; set; }
        public int SectionStatus { get; set; }
    }
}
