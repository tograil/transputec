using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessages {
    public class GetMessagesHandler : IRequestHandler<GetMessagesRequest, GetMessagesResponse> {
        private readonly IMessageQuery _messageQuery;
        private readonly GetMessagesValidator _getMessagesValidator;

        public GetMessagesHandler(GetMessagesValidator getMessagesValidator,
            IMessageQuery messageQuery) {
            _messageQuery = messageQuery;
            _getMessagesValidator = getMessagesValidator;
        }

        public async Task<GetMessagesResponse> Handle(GetMessagesRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetMessagesRequest));
            await _getMessagesValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _messageQuery.GetMessages(request);
            return result;
        }
    }
}
