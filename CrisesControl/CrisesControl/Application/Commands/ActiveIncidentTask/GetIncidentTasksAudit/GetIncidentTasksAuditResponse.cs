using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit
{
    public class GetIncidentTasksAuditResponse
    {
        public List<IncidentTaskAudit> Data { get; set; }
        public CrisesControl.Core.Incidents.Incidents Incident { get; set; }
    }
}
