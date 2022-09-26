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
        private readonly ILogger<GetUserDashboardHandler> _logger;

        public GetUserDashboardHandler(GetUserDashboardValidator userValidator, IUserRepository userService, ILogger<GetUserDashboardHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<GetUserDashboardResponse> Handle(GetUserDashboardRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserDashboardRequest));

            var userId = await _userRepository.GetUserDashboard(request.ModulePage, request.UserID, request.Reverse);
            return new GetUserDashboardResponse();
        }
    }
}
