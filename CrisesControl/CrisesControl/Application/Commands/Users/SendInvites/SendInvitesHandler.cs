using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SendInvites
{
    public class SendInvitesHandler: IRequestHandler<SendInvitesRequest, SendInvitesResponse>
    {
        private readonly SendInvitesValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SendInvitesHandler> _logger;

        public SendInvitesHandler(SendInvitesValidator userValidator, IUserRepository userService, ILogger<SendInvitesHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<SendInvitesResponse> Handle(SendInvitesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendInvitesRequest));
            
            var message = await _userRepository.SendInvites(cancellationToken);
            var result = new SendInvitesResponse();
            result.Message = message;
            return result;
        }

        private bool CheckDuplicate(User user)
        {
            return _userRepository.CheckDuplicate(user);
        }
    }
}
