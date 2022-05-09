using CrisesControl.Core.Communication;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences {
    public class GetUserActiveConferencesResponse {
        public List<UserConferenceItem> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
