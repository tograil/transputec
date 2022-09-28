using CrisesControl.Core.Incidents;
using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTimeline
{
    public class GetIncidentTimelineResponse
    {
        public List<IncidentMessagesRtn> Data { get; set; }
    }
}
