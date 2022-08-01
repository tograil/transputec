using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData
{
    public class GetUserReportPiechartDataResponse
    {
        public List<UserPieChart> data { get; set; }
        public string Message { get; set; }
    }
}
