using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetCompanyCommunicationReport
{
    public class GetCompanyCommunicationReportResponse
    {
        public int CompanyId { get; set; }
        public long TotalPushCount { get; set; }
        public long TotalEmailCount { get; set; }
        public long TotalTextCount { get; set; }
        public long TotalPhoneCount { get; set; }
        public List<CompanyUserCountReturn> CompanyUserCountReturn { get; set; }
    }
}
