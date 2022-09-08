using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.CCPhoneNumbers
{
    public class CCPhoneNumbersHandler : IRequestHandler<CCPhoneNumbersRequest, CCPhoneNumbersResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<CCPhoneNumbersHandler> _logger;
        public CCPhoneNumbersHandler(IAppQuery appQuery, ILogger<CCPhoneNumbersHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<CCPhoneNumbersResponse> Handle(CCPhoneNumbersRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.CCPhoneNumbers(request);
            return result;
        }
    }
}
