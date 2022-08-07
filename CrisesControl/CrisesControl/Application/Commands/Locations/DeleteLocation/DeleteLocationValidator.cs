using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.DeleteLocation
{
    public class DeleteLocationValidator:AbstractValidator<DeleteLocationRequest>
    {
        public DeleteLocationValidator()
        {
            RuleFor(x => x.LocationId).GreaterThan(0);
        }
    }
}
