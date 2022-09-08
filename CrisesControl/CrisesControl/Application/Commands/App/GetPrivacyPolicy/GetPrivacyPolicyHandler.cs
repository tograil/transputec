using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.GetPrivacyPolicy
{
    public class GetPrivacyPolicyHandler:IRequestHandler<GetPrivacyPolicyRequest, GetPrivacyPolicyResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<GetPrivacyPolicyHandler> _logger;
        public GetPrivacyPolicyHandler(IAppQuery appQuery, ILogger<GetPrivacyPolicyHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }

        public async Task<GetPrivacyPolicyResponse> Handle(GetPrivacyPolicyRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.GetPrivacyPolicy(request);
            return result;
        }
    }
}
