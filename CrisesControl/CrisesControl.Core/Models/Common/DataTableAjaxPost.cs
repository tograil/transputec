using CrisesControl.Core.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models.Common
{
    public class DataTableAjaxPost
    {
        public DataTableAjaxPost()
        {
            Filters = "";
            UniqueKey = "";
        }
        public bool SkipDeleted { get; set; }
        public bool ActiveOnly { get; set; }
        public bool SkipInActive { get; set; }
        public bool KeyHolderOnly { get; set; }
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        [NotMapped]
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public string UniqueKey { get; set; }
        public string Filters { get; set; }
    }
}
