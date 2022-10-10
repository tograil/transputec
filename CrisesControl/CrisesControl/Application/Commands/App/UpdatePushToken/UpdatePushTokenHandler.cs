using MediatR;
using CrisesControl.Api.Application.Query;

namespace CrisesControl.Api.Application.Commands.App.UpdatePushToken
{
    public class UpdatePushTokenHandler : IRequestHandler<UpdatePushTokenRequest, UpdatePushTokenResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<UpdatePushTokenHandler> _logger;
        public UpdatePushTokenHandler(IAppQuery appQuery, ILogger<UpdatePushTokenHandler> logger)
        {
            _appQuery = appQuery;
            _logger = logger;
        }
        public async Task<UpdatePushTokenResponse> Handle(UpdatePushTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.UpdatePushToken(request);
            return result;
        }
    }
}
