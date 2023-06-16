using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SendPasswordOTP
{
    public class SendPasswordOTPHandler : IRequestHandler<SendPasswordOTPRequest, SendPasswordOTPResponse>
    {
        private readonly SendPasswordOTPValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SendPasswordOTPHandler> _logger;

        public SendPasswordOTPHandler(SendPasswordOTPValidator userValidator, IUserRepository userService, ILogger<SendPasswordOTPHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<SendPasswordOTPResponse> Handle(SendPasswordOTPRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendPasswordOTPRequest));
            await _userValidator.ValidateAndThrowAsync(request,cancellationToken);
            var PasswordOtp = await _userRepository.SendPasswordOTP(request.UserID,request.Action,request.Password,request.OldPassword,request.OTPCode,request.Return,request.OTPMessage,request.Source,request.Method);
            var response= new SendPasswordOTPResponse();
            response.Message = PasswordOtp;
            return response;
        }
    }
}
