using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class DataMappingColumn
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public bool isRequired { get; set; }
        public string DataType { get; set; }
        public int ColumnIndex { get; set; }
    }
}
