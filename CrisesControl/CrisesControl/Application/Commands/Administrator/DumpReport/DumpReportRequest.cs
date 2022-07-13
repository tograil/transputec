using CrisesControl.Core.Administrator;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DumpReport
{
    public class DumpReportRequest:IRequest<DumpReportResponse>
    {
        public int ReportID { get; set; }
        public List<ReportParam> ParamList { get; set; }
        public bool DownloadFile { get; set; }
    }
}
