using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserRequest, GetAllUserResponse>
    {
        private readonly GetAllUserValidator _userValidator;
        private readonly IUserQuery _userRepository;
        private readonly ILogger<GetAllUserHandler> _logger;

        public GetAllUserHandler(GetAllUserValidator userValidator, IUserQuery userService, ILogger<GetAllUserHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<GetAllUserResponse> Handle(GetAllUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllUserRequest));
            await _userValidator.ValidateAndThrowAsync(request,cancellationToken);
            var response = await _userRepository.GetUsers(request,cancellationToken);
            return response;
        }
    }
}
