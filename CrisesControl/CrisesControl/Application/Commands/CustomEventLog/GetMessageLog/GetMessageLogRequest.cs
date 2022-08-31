using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetMessageLog
{
    public class GetMessageLogRequest : IRequest<GetMessageLogResponse>
    {
        public int EventLogId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
