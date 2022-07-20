using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class LocationOnlyUploadModel
    {
        public string SessionId { get; set; }
        public int UserImportTotalId { get; set; }
        public LocationUploadData[] LocationUploadData { get; set; }
    }

    public class LocationUploadData
    {
        public int UserImportTotalId { get; set; }
        public string ImportAction { get; set; }
    }
}
