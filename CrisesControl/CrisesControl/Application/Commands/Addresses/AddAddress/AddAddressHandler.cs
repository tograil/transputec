using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.AddAddress
{
    public class AddAddressHandler : IRequestHandler<AddAddressRequest, AddAddressResponse>
    {
        private readonly IAddressQuery _addressQuery;
        private readonly ILogger<AddAddressHandler> _logger;
        public AddAddressHandler(IAddressQuery addressQuery, ILogger<AddAddressHandler> logger)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
        }
        public async Task<AddAddressResponse> Handle(AddAddressRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AddAddressRequest));
            var result = await _addressQuery.AddAddress(request);
            return result;
        }
    }
}
