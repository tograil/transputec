using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAddress
{
    public class GetAddressHandler:IRequestHandler<GetAddressRequest, GetAddressResponse>
    {
        private readonly IAddressQuery _addressQuery;
        private readonly ILogger<GetAddressHandler> _logger;
        private readonly GetAddressValidator _getAddressValidator;
        public GetAddressHandler(IAddressQuery addressQuery, ILogger<GetAddressHandler> logger, GetAddressValidator getAddressValidator)
        {
            this._addressQuery = addressQuery;
            this._logger = logger;
            this._getAddressValidator = getAddressValidator;
        }

        public async Task<GetAddressResponse> Handle(GetAddressRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAddressRequest));
            await _getAddressValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _addressQuery.GetAdress(request);
            return result;
        }
    }
}
