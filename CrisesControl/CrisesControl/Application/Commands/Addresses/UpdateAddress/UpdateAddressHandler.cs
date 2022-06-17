using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.UpdateAddress
{
    public class UpdateAddressHandler:IRequestHandler<UpdateAddressRequest, UpdateAddressResponse>
    {
        private readonly IAddressQuery _addressQuery;
        private readonly ILogger<UpdateAddressHandler> _logger;
        private readonly UpdateAddressValidator _updateAddressValidator;
        public UpdateAddressHandler(IAddressQuery addressQuery, ILogger<UpdateAddressHandler> logger, UpdateAddressValidator updateAddressValidator)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
            this._updateAddressValidator = updateAddressValidator;
        }

        public async Task<UpdateAddressResponse> Handle(UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateAddressRequest));
            await _updateAddressValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _addressQuery.UpdateAddress(request);
            return result;
        }
    }
}
