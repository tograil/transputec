using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserSystemInfo
{
    public class GetUserSystemInfoHandler : IRequestHandler<GetUserSystemInfoRequest, GetUserSystemInfoResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserSystemInfoHandler> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public GetUserSystemInfoHandler( IUserRepository userService, ILogger<GetUserSystemInfoHandler> logger, ICurrentUser currentUser, IMapper mapper)
        {
          
            _userRepository = userService;
            _logger = logger;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<GetUserSystemInfoResponse> Handle(GetUserSystemInfoRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserSystemInfoRequest));

            var userParams = await _userRepository.GetUserSystemInfo(_currentUser.UserId, _currentUser.CompanyId);
            var result = _mapper.Map<List<UserParams>>(userParams);
            var response = new GetUserSystemInfoResponse();
            response.Data = result;
            return response;

        }
    }
}
