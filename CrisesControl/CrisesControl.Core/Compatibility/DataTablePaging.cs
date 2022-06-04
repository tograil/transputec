using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Compatibility
{
    public class DataTablePaging
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        [NotMapped] 
        public object Data { get; set; }
    }
}
