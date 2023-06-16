using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReportDetails
{
    public class GetIncidentReportDetailsRequest:IRequest<GetIncidentReportDetailsResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
