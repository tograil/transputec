using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.SendCredentials
{
    public class SendCredentialsHandler : IRequestHandler<SendCredentialsRequest, SendCredentialsResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly SendCredentialsValidator _sendCredentialsValidator;
        public SendCredentialsHandler()
        {

        }
        public  async Task<SendCredentialsResponse> Handle(SendCredentialsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendCredentialsRequest));

            await _sendCredentialsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _registerQuery.SendCredentials(request);
            return result;
        }
    }
}
