using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesHandler : IRequestHandler<GetRepliesRequest, GetRepliesResponse>
    {
        private readonly IMessageQuery _messageQuery;
        public GetRepliesHandler(IMessageQuery messageQuery)
        {
          this._messageQuery = messageQuery;
        }
        public async Task<GetRepliesResponse> Handle(GetRepliesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetRepliesRequest));
            var result = await  _messageQuery.GetReplies(request);
            return result;
        }
    }
}
