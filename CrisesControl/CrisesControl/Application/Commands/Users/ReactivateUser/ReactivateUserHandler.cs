using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ReactivateUser
{
    public class ReactivateUserHandler : IRequestHandler<ReactivateUserRequest, ReactivateUserResponse>
    {
        private readonly ReactivateUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ReactivateUserHandler> _logger;

        public ReactivateUserHandler(ReactivateUserValidator userValidator, IUserRepository userService, ILogger<ReactivateUserHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<ReactivateUserResponse> Handle(ReactivateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReactivateUserRequest));
            await _userValidator.ValidateAndThrowAsync(request,cancellationToken);
            var user = await _userRepository.ReactivateUser(request.QueriedUserId, cancellationToken);
            var response= new ReactivateUserResponse();
            response.User = user;
            return response;
        }
    }
}
