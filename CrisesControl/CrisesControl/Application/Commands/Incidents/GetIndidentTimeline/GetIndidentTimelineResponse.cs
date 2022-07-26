using CrisesControl.Core.Incidents;
using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline
{
    public class GetIndidentTimelineResponse
    {
        public List<IncidentMessagesRtn> Data { get; set; }
    }
}
