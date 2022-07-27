using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop
{
    public class LibraryText
    {
        public string CompanyName { get; set; }
        public int LibSOPHeaderID { get; set; }
        public int LibSectionID { get; set; }
        public int LibContentID { get; set; }
        public string SectionDescription { get; set; }
        public string SectionName { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int TotalVotes { get; set; }
        public int NoOfUse { get; set; }
        public int TotalRating { get; set; }
        [NotMapped]
        public UserFullName Author { get; set; }
        [NotMapped]
        public List<int> SOPContentTags { get; set; }
    }
}
