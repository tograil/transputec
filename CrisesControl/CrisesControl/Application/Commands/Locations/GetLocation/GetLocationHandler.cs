using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocation
{
    public class GetLocationHandler:IRequestHandler<GetLocationRequest, GetLocationResponse>
    {
        private readonly ILocationQuery _locationQuery;
        private readonly ILogger<GetLocationHandler> _logger;
        private readonly GetLocationValidator _getLocationValidator;
        public GetLocationHandler(ILocationQuery locationRepository, GetLocationValidator getLocationValidator,  ILogger<GetLocationHandler> logger)
        {
            this._locationQuery = locationRepository;
            this._logger = logger;
          
            this._getLocationValidator = getLocationValidator;
        }

        public async Task<GetLocationResponse> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
          
                Guard.Against.Null(request, nameof(GetLocationRequest));
                await _getLocationValidator.ValidateAndThrowAsync(request, cancellationToken);
                var location = await _locationQuery.GetLocation(request,cancellationToken);
                //var result = _mapper.Map<List<GroupLink>>(location);
                
                return location;
           
        }
    }
}
