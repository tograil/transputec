using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using FluentValidation;
using MediatR;
using System.Security.Claims;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses {
    public class GetMessageResponsesHandler : IRequestHandler<GetMessageResponsesRequest, GetMessageResponsesResponse> {
        private readonly IMessageQuery _messageQuery;
        private readonly IMessageRepository _messageRepository;
        private readonly GetMessageResponsesValidator _getMessageResponseValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string TimeZoneId = "GMT Standard Time";

        public GetMessageResponsesHandler(GetMessageResponsesValidator getMessageResponseValidator,
            IMessageQuery messageQuery,
            IMessageRepository messageRepository,
            IHttpContextAccessor httpContextAccessor) {
            _getMessageResponseValidator = getMessageResponseValidator;
            _messageQuery = messageQuery;
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetMessageResponsesResponse> Handle(GetMessageResponsesRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetMessageResponsesRequest));
            await _getMessageResponseValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _messageQuery.GetMessageResponses(request);

            if (result.Data.Count <= 0) {
                int UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                int CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                await _messageRepository.CopyMessageResponse(CompanyID, UserID, TimeZoneId, cancellationToken);

                var result_again = await _messageQuery.GetMessageResponses(request);

                return result_again;
            } else {
                return result;
            }
        }
    }
}
