using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.GetIncidentReportDetails
{
    public class GetIncidentReportDetailsRequest : IRequest<GetIncidentReportDetailsResponse>
    {
        public int IncidentActivationId { get; set; }
        public int CompanyId {get;set;}
        public int UserId {get;set;}

    }
}
