using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentLibrary
{
    public class GetIncidentLibraryResponse
    {
        public List<IncidentLibraryDetails> Data { get; set; }
        public string Message { get; set; }
    }
}
