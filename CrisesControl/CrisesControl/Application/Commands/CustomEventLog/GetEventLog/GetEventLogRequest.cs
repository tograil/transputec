using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog
{
    public class GetEventLogRequest : IRequest<GetEventLogResponse>
    {
        public int EventLogId { get; set; }
        public int EventLogHeaderId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
