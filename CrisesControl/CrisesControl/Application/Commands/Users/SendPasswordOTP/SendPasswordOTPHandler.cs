using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SendPasswordOTP
{
    public class SendPasswordOTPHandler : IRequestHandler<SendPasswordOTPRequest, SendPasswordOTPResponse>
    {
        private readonly SendPasswordOTPValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SendPasswordOTPHandler(SendPasswordOTPValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<SendPasswordOTPResponse> Handle(SendPasswordOTPRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendPasswordOTPRequest));

            //var userId = await _userRepository.SendPasswordOTP(request.ModuleItems, request.ModulePage, request.UserID, cancellationToken);
            return new SendPasswordOTPResponse();
        }
    }
}
