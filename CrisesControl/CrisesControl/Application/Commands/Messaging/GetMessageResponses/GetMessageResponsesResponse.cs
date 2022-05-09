using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses {
    public class GetMessageResponsesResponse {
        public List<CompanyMessageResponse> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
