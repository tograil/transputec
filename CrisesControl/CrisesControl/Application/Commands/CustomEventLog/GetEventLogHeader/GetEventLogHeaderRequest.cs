using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLogHeader
{
    public class GetEventLogHeaderRequest : IRequest<GetEventLogHeaderResponse>
    {

        public int ActiveIncidentId { get; set; }
        public int EventLogHeaderId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
