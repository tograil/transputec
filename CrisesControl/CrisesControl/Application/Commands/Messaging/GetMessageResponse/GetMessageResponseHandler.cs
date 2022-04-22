using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse {
    public class GetMessageResponseHandler : IRequestHandler<GetMessageResponseRequest, GetMessageResponseResponse> {
        private readonly IMessageQuery _messageQuery;
        private readonly GetMessageResponseValidator _getMessageResponseValidator;

        public GetMessageResponseHandler(GetMessageResponseValidator getMessageResponseValidator,
            IMessageQuery messageQuery) {
            _messageQuery = messageQuery;
            _getMessageResponseValidator = getMessageResponseValidator;
        }

        public async Task<GetMessageResponseResponse> Handle(GetMessageResponseRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetMessageResponseRequest));
            await _getMessageResponseValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _messageQuery.GetMessageResponse(request);
            return result;
        }
    }
}
