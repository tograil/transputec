using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetUnpaidTransactions
{
    public class GetUnpaidTransactionsHandler : IRequestHandler<GetUnpaidTransactionsRequest, GetUnpaidTransactionsResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetUnpaidTransactionsHandler> _logger;

        public GetUnpaidTransactionsHandler(IAdminQuery adminQuery, ILogger<GetUnpaidTransactionsHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;

        }
        public async Task<GetUnpaidTransactionsResponse> Handle(GetUnpaidTransactionsRequest request, CancellationToken cancellationToken)
        {
            var UnpaidTrans = await _adminQuery.GetUnpaidTransactions(request);
            return UnpaidTrans;
        }
    }
}
