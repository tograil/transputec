using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class GroupOnlyImportResult
    {
        public int UserImportTotalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string GroupName { get; set; }
        public string GroupStatus { get; set; }
        public string GroupCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
        public string ErrorId { get; set; }
        public string Message { get; set; }
        public string MyProperty { get; set; }
    }
}
