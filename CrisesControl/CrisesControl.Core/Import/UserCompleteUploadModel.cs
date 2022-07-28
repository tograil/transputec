using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class UserCompleteUploadModel
    {

        public bool ImportAsActive { get; set; }
        public UserCompleteUploadData[] UserCompleteUploadData { get; set; }
        public int UserImportTotalId { get; set; }
        public string SessionId { get; set; }
    }

    public class UserCompleteUploadData
    {
        public int UserImportTotalId { get; set; }
        public string ImportAction { get; set; }
    }
}
