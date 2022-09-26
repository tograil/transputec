using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserCount
{
    public class GetUserCountHandler : IRequestHandler<GetUserCountRequest, GetUserCountResponse>
    {
        private readonly GetUserCountValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserCountHandler> _logger;

        public GetUserCountHandler(GetUserCountValidator userValidator, IUserRepository userService, ILogger<GetUserCountHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<GetUserCountResponse> Handle(GetUserCountRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserCountRequest));

            var userId = await _userRepository.GetUserCount(request.CompanyId, request.UserId);
            return new GetUserCountResponse();
        }
    }
}
