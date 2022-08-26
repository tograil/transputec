using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetLogs
{
    public class GetLogsRequest : IRequest<GetLogsResponse>
    {
        public int ActiveIncidentId { get; set; }
        public int EventLogHeaderId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
