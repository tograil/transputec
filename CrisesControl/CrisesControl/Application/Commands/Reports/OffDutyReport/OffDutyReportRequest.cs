using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.OffDutyReport
{
    public class OffDutyReportRequest:IRequest<OffDutyReportResponse>
    {
        public int CompanyId { get; set; }
    }
}
