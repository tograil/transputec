using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.CreateLocation
{
    public class CreateLocationValidator: AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationValidator()
        {
            RuleFor(x => x.LocationId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
