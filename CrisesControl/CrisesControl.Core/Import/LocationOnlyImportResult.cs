using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class LocationOnlyImportResult
    {
        public int UserImportTotalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string LocationStatus { get; set; }
        public string LocationCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
    }
}
