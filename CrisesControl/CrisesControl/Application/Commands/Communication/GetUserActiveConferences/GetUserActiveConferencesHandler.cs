using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences {
    public class GetUserActiveConferencesHandler : IRequestHandler<GetUserActiveConferencesRequest, GetUserActiveConferencesResponse> {
        private readonly ICommunicationQuery _communicationQuery;

        public GetUserActiveConferencesHandler(ICommunicationQuery communicationQuery) {
            _communicationQuery = communicationQuery;
        }

        public async Task<GetUserActiveConferencesResponse> Handle(GetUserActiveConferencesRequest request, CancellationToken cancellationToken) {

            var result = await _communicationQuery.GetUserActiveConferences(request);
            return result;
        }
    }
}
