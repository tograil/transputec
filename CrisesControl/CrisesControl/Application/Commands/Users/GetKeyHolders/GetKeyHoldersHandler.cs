using CrisesControl.Core.Users.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;
using CrisesControl.Core.Users;
using AutoMapper;

namespace CrisesControl.Api.Application.Commands.Users.GetKeyHolders
{
    public class GetKeyHoldersHandler : IRequestHandler<GetKeyHoldersRequest, GetKeyHoldersResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        public GetKeyHoldersHandler(IUserRepository userRepository, ICurrentUser currentUser, IMapper mapper)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
           _mapper = mapper;
        }
        public async Task<GetKeyHoldersResponse> Handle(GetKeyHoldersRequest request, CancellationToken cancellationToken)
        {
            var response = new GetKeyHoldersResponse();
            var keyHolders = await _userRepository.GetKeyHolders(_currentUser.CompanyId);
            var result = _mapper.Map<List<KeyHolderResponse>>(keyHolders);
            response.Data = result;
            return response;
        }
    }
}
