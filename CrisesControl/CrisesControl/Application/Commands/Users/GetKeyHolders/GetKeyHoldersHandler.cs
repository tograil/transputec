using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetKeyHolders
{
    public class GetKeyHoldersHandler : IRequestHandler<GetKeyHoldersRequest, GetKeyHoldersResponse>
    {
        private readonly GetKeyHoldersValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly Logger<GetKeyHoldersHandler> _logger;

        public GetKeyHoldersHandler(GetKeyHoldersValidator userValidator, IUserRepository userService, Logger<GetKeyHoldersHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<GetKeyHoldersResponse> Handle(GetKeyHoldersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetKeyHoldersRequest));
            var response = new GetKeyHoldersResponse();
            response.Data = await _userRepository.GetKeyHolders(request.UserId);
            return response;
        }
    }
}
