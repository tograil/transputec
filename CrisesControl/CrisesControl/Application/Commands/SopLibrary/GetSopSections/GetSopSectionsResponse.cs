using CrisesControl.Core.SopLibrary;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSections
{
    public class GetSopSectionsResponse
    {
        public List<SopSectionList> SopSection { get; set; }
        public string Message { get; set; }
    }
}
