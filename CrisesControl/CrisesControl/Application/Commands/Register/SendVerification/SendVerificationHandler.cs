using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.SendVerification
{
    public class SendVerificationHandler : IRequestHandler<SendVerificationRequest, SendVerificationResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly SendVerificationValidator _sendVerificationValidator;
        public SendVerificationHandler(IRegisterQuery registerQuery, SendVerificationValidator sendVerificationValidator)
        {
            _registerQuery = registerQuery;
            _sendVerificationValidator = sendVerificationValidator;
        }
        public async Task<SendVerificationResponse> Handle(SendVerificationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendVerificationRequest));

            await _sendVerificationValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _registerQuery.SendVerification(request);
            return result;
        }
    }
}
