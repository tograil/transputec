using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.GetIncidentData
{
    public class GetIncidentDataRequest : IRequest<GetIncidentDataResponse>
    {
        public int IncidentActivationId { get; set; }
        public int CompanyId { get; set; }
    }
}
