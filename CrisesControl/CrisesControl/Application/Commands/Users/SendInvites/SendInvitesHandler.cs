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
        private readonly IMapper _mapper;

        public SendInvitesHandler(SendInvitesValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
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
