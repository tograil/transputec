using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails
{
    public class GetIndidentReportDetailsRequest:IRequest<GetIndidentReportDetailsResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
