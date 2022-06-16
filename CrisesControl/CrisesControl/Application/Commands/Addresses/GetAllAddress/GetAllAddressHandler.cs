using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAllAddress
{
    public class GetAllAddressHandler : IRequestHandler<GetAllAddressRequest, GetAllAddressResponse>
    {
        private readonly IAddressQuery _addressQuery;
        private readonly ILogger<GetAllAddressHandler> _logger;
        public GetAllAddressHandler(IAddressQuery addressQuery, ILogger<GetAllAddressHandler> logger)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
        }

        public Task<GetAllAddressResponse> Handle(GetAllAddressRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllAddressRequest));
            var result = _addressQuery.GetAllAddress(request);
            return result;
        }
    }
}
