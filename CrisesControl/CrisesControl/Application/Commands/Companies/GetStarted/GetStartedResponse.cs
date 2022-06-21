using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.GetStarted
{
    public class GetStartedResponse
    {
        public GetCompanyDataResponse Data { get; set; }
        public string Message { get; set; }
    }
}
