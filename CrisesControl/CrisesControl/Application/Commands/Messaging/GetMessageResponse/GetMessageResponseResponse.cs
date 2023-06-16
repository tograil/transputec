using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse {
    public class GetMessageResponseResponse {
        public CompanyMessageResponse Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
