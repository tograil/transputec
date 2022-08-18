using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMapLocations
{
    public class GetIncidentMapLocationsValidator:AbstractValidator<GetIncidentMapLocationsRequest>
    {
        public GetIncidentMapLocationsValidator()
        {
            RuleFor(x => x.ActiveIncidentId).GreaterThan(0);
        }
    }
}
