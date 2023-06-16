using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes
{
    public class GetIncidentTaskNotesResponse
    {
        public List<IncidentTask> result { get; set; }
        public string  Message { get; set; }

    }
}
