using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.DeleteAddress
{
    public class DeleteAddressHandler : IRequestHandler<DeleteAddressRequest, DeleteAddressResponse>
    {
        private readonly ILogger<DeleteAddressHandler> _logger;
        private readonly IAddressQuery _addressQuery;
        private readonly DeleteAddressValidator _deleteAddressValidator;
        public DeleteAddressHandler(ILogger<DeleteAddressHandler> logger, IAddressQuery addressQuery, DeleteAddressValidator deleteAddressValidator)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
            this._deleteAddressValidator = deleteAddressValidator;
        }
        public async Task<DeleteAddressResponse> Handle(DeleteAddressRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteAddressRequest));
            await _deleteAddressValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _addressQuery.DeleteAddress(request);
            return result;
        }
    }
}
