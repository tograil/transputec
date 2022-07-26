using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline
{
    public class GetIndidentTimelineResponse
    {
        public List<IncidentMessagesRtn> Data { get; set; }
    }
}
