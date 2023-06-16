using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class GetAllUserRequestList
    {
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string SearchString { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public bool SkipDeleted { get; set; }
        public bool ActiveOnly { get; set; }
        public bool SkipInActive { get; set; }
        public bool KeyHolderOnly { get; set; }
        public string Filters { get; set; }
        public string CompanyKey { get; set; }
    }
}
