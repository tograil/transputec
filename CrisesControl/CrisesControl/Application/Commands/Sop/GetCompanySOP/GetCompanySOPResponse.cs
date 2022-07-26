using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Sop.GetCompanySOP
{
    public class GetCompanySOPResponse
    {
        public List<LibSopSection> data { get; set; }
        public string Message { get; set; }
    }
}
