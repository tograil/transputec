using CrisesControl.Core.Incidents;
using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask
{
    public class UnattendedTaskResponse
    {
        public List<FailedTaskList> Data { get; set; }
        public string Message { get; set; }
    }
}
