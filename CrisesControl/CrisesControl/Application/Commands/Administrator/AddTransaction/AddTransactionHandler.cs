using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddTransaction
{
    public class AddTransactionHandler : IRequestHandler<AddTransactionRequest, AddTransactionResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<AddTransactionHandler> _logger;

        public AddTransactionHandler(IAdminQuery adminQuery, ILogger<AddTransactionHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<AddTransactionResponse> Handle(AddTransactionRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.AddTransaction(request);
            return result;
        }
    }
}
