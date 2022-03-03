using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocation
{
    public class GetLocationValidator: AbstractValidator<GetLocationRequest>
    {
        public GetLocationValidator()
        {
            RuleFor(x => x.LocationId)
                .GreaterThan(0);

        }
    }
}
