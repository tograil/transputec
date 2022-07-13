using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTransactionType
{
    public class GetTransactionTypeHandler : IRequestHandler<GetTransactionTypeRequest, GetTransactionTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetTransactionTypeHandler> _logger;

        public GetTransactionTypeHandler(IAdminQuery adminQuery, ILogger<GetTransactionTypeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;

        }
        public async Task<GetTransactionTypeResponse> Handle(GetTransactionTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetTransactionType(request);
            return result;
        }
    }
}
