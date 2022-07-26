using CrisesControl.Core.Sop;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSections
{
    public class GetSopSectionsResponse
    {
        public ContentSectionData data { get; set; }
        public string Message { get; set; }
    }
}
