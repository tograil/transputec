using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class UserImportDataValidation
    {
        public string FieldName { get; set; }
        public bool MandatoryError { get; set; }
        public bool DataError { get; set; }
        public string Message { get; set; }
    }
}
