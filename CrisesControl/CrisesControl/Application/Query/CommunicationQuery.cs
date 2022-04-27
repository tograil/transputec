using AutoMapper;
using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class CommunicationQuery : ICommunicationQuery {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IMapper _mapper;
        public CommunicationQuery(ICommunicationRepository communicationRepository, IMapper mapper)
        {
            this._communicationRepository = communicationRepository;
            this._mapper = mapper;

        public CommunicationQuery(ICommunicationRepository communicationRepository, IMapper mapper,
           ILogger<BillingQuery> logger) {
            _mapper = mapper;
            _communicationRepository = communicationRepository;
        }

        public async Task<GetUserActiveConferencesResponse> GetUserActiveConferences(GetUserActiveConferencesRequest request) {
            var conflist = await _communicationRepository.GetUserActiveConferences();
            var response = _mapper.Map<List<UserConferenceItem>>(conflist);
            var result = new GetUserActiveConferencesResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }
    }
}
