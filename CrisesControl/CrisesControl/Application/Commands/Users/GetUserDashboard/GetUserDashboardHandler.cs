using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserDashboard
{
    public class GetUserDashboardHandler : IRequestHandler<GetUserDashboardRequest, GetUserDashboardResponse>
    {
        private readonly GetUserDashboardValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserDashboardHandler(GetUserDashboardValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<GetUserDashboardResponse> Handle(GetUserDashboardRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserDashboardRequest));

            var userId = await _userRepository.GetUserDashboard(request.ModulePage, request.UserID, request.Reverse);
            return new GetUserDashboardResponse();
        }
    }
}
