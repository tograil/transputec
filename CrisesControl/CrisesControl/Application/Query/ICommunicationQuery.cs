using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences;

namespace CrisesControl.Api.Application.Query {
    public interface ICommunicationQuery {
        public Task<GetUserActiveConferencesResponse> GetUserActiveConferences(GetUserActiveConferencesRequest request);
    }
}
