using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetReport
{
    public class GetReportRequest:IRequest<GetReportResponse>
    {
        public int ReportId { get; set; }
    }
}
