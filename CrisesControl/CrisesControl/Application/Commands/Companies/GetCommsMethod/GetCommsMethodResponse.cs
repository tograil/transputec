using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Companies.GetCommsMethod {
    public class GetCommsMethodResponse {
        public List<CommsMethod> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
