using AutoMapper;
using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public class CommunicationQuery : ICommunicationQuery
    {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IMapper _mapper;
        public CommunicationQuery(ICommunicationRepository communicationRepository, IMapper mapper)
        {
            this._communicationRepository = communicationRepository;
            this._mapper = mapper;

        }
        public async Task<GetUserActiveConferenceListResponse> GetUserActiveConferenceList(GetUserActiveConferenceListRequest request)
        {
            var conferences = await _communicationRepository.GetUserActiveConferenceList(request.UserID, request.CompanyID);
            List<GetUserActiveConferenceListResponse> response = _mapper.Map<List<ConferenceDetails>, List<GetUserActiveConferenceListResponse>>(conferences.ToList());
            var result = new GetUserActiveConferenceListResponse();
            result.Data = response;
            return result;
        }
    }
}
