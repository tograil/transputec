using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.ExportEventLog
{
    public class ExportEventLogRequest : IRequest<ExportEventLogResponse>
    {
        public int EventLogHeaderId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int PermittedDepartment { get; set; }
    }
}
