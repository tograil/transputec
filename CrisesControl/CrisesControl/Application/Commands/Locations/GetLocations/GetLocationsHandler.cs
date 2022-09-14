using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsHandler:IRequestHandler<GetLocationsRequest,GetLocationsResponse>
    {
        private readonly GetLocationsValidator _getLocationsValidator;
        private readonly ILocationQuery _locationQuery;
        private readonly ILogger<GetLocationsHandler> _logger;
       
        public GetLocationsHandler(ILocationQuery locationRepository, GetLocationsValidator getLocationsValidator, ILogger<GetLocationsHandler> logger)
        {
            this._locationQuery = locationRepository;
            this._logger = logger;
            this._getLocationsValidator = getLocationsValidator;
        }

        public async Task<GetLocationsResponse> Handle(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationsRequest));
            await _getLocationsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var location = await _locationQuery.GetLocations(request, cancellationToken);
            //var result = _mapper.Map<List<GroupLink>>(location);

            return location;
        }
    }
}
