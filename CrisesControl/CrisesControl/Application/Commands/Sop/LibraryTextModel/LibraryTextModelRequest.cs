using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.LibraryTextModel
{
    public class LibraryTextModelRequest:IRequest<LibraryTextModelResponse>
    {
        public string IncidentName { get; set; }
        public string IncidentType { get; set; }
        public string Sector { get; set; }
        public string SectionTitle { get; set; }
    }
}
