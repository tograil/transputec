using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit
{
    public class GetTaskAuditResponse
    {
        public List<TaskAudit> Data { get; set; }
        public string Message { get; set; }
    }
}
