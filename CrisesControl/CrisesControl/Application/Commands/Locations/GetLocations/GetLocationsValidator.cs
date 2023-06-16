using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsValidator: AbstractValidator<GetLocationsRequest>
    {
        public GetLocationsValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
