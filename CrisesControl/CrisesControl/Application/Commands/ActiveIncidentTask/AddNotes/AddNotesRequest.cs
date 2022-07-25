using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes
{
    public class AddNotesRequest:IRequest<AddNotesResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskCompletionNote { get; set; }
    }
}
