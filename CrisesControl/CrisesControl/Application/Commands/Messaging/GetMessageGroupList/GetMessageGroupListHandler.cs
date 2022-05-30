using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList
{
    public class GetMessageGroupListHandler : IRequestHandler<GetMessageGroupListRequest, GetMessageGroupListResponse>
    {
        private readonly IMessageQuery _messageQuery;
        private readonly ILogger<GetMessageGroupListHandler> _logger;
        private readonly GetMessageGroupListValidator _getMessageGroupListValidator;   
        public GetMessageGroupListHandler(IMessageQuery messageQuery, ILogger<GetMessageGroupListHandler> logger, GetMessageGroupListValidator getMessageGroupListValidator)
        {
            this._messageQuery = messageQuery;
            this._logger = logger; 
            this._getMessageGroupListValidator = getMessageGroupListValidator;
        }
        public async Task<GetMessageGroupListResponse> Handle(GetMessageGroupListRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetMessageGroupListRequest));
            await _getMessageGroupListValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _messageQuery.GetMessageGroupList(request);
            return result;
        }
    }
}
