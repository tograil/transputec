using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.SegregationLinks
{
    public class SegregationLinksResponse
    {
        public IncidentSegLinks Data { get; set; }
        public string Message { get; set; }
    }
}
