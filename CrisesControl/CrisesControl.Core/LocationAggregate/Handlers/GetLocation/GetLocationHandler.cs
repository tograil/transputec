using Ardalis.GuardClauses;
using CrisesControl.Core.LocationAggregate.Services;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate.Handlers.GetLocation
{
    public class GetLocationHandler: IRequestHandler<GetLocationRequest, GetLocationResponse>
    {
        private readonly GetLocationValidator _locationValidator;
        private readonly ILocationService _locationService;

        public GetLocationHandler(GetLocationValidator locationValidator, ILocationService locationService)
        {
            _locationService = locationService;
            _locationValidator = locationValidator;
        }
        public async Task<GetLocationResponse> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationRequest));

            await _locationValidator.ValidateAndThrowAsync(request, cancellationToken);

            var groups = await _locationService.GetAllLocations();

            return new GetLocationResponse();
        }
    }
}
