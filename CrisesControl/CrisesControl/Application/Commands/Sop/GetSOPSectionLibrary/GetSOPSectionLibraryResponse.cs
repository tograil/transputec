using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Sop.GetSOPSectionLibrary
{
    public class GetSOPSectionLibraryResponse
    {
        public List<LibSopSection> Data { get; set; }
        public string Message { get; set; }
    }
}
