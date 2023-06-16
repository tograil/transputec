using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ResetUserDeviceToken
{
    public class ResetUserDeviceTokenHandler:IRequestHandler<ResetUserDeviceTokenRequest, ResetUserDeviceTokenResponse>
    {
        private readonly ResetUserDeviceTokenValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ResetUserDeviceTokenHandler(ResetUserDeviceTokenValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<ResetUserDeviceTokenResponse> Handle(ResetUserDeviceTokenRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ResetUserDeviceTokenRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            await _userRepository.ResetUserDeviceToken(request.UserId);
            var response = new ResetUserDeviceTokenResponse();
            response.Token = true;
            response.Message = "Token reseted";
            return response;
        }
    }
}
