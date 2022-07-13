using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction
{
    public class GetCompanyTransactionHandler : IRequestHandler<GetCompanyTransactionRequest, GetCompanyTransactionResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetCompanyTransactionHandler> _logger;

        public GetCompanyTransactionHandler(IAdminQuery adminQuery, ILogger<GetCompanyTransactionHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;

        }
        public async Task<GetCompanyTransactionResponse> Handle(GetCompanyTransactionRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetCompanyTransaction(request);
            return result;
        }
    }
}
