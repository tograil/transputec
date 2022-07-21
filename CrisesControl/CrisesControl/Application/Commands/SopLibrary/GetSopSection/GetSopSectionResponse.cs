using CrisesControl.Core.SopLibrary;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection
{
    public class GetSopSectionResponse
    {
        public SopSection SopSection { get; set; }
        public string Message { get; set; }
    }
}
