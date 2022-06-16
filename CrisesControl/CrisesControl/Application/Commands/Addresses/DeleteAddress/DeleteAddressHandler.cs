using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.DeleteAddress
{
    public class DeleteAddressHandler : IRequestHandler<DeleteAddressRequest, DeleteAddressResponse>
    {
        private readonly ILogger<DeleteAddressHandler> _logger;
        private readonly IAddressQuery _addressQuery;
        public DeleteAddressHandler(ILogger<DeleteAddressHandler> logger, IAddressQuery addressQuery)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
        }
        public async Task<DeleteAddressResponse> Handle(DeleteAddressRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
