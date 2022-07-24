using FluentValidation;

namespace CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText
{
    public class UseLibraryTextValidator:AbstractValidator<UseLibraryTextRequest>
    {
        public UseLibraryTextValidator()
        {
            RuleFor(x => x.SOPHeaderID).GreaterThan(0);
        }
    }
}
