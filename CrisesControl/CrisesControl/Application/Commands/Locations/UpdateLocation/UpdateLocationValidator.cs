using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateLocation
{
    public class UpdateLocationValidator: AbstractValidator<UpdateLocationRequest>
    {
        public UpdateLocationValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
