using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.ReferToFriend
{
    public class ReferToFriendHandler : IRequestHandler<ReferToFriendRequest, ReferToFriendResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<ReferToFriendHandler> _logger;
        public ReferToFriendHandler(IAppQuery appQuery, ILogger<ReferToFriendHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<ReferToFriendResponse> Handle(ReferToFriendRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.ReferToFriend(request);
            return result;
        }
    }
}
