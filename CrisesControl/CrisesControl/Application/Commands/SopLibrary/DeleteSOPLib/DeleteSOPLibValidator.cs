using FluentValidation;

namespace CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib
{
    public class DeleteSOPLibValidator:AbstractValidator<DeleteSOPLibRequest>
    {
        public DeleteSOPLibValidator()
        {
            RuleFor(x => x.SOPHeaderID).GreaterThan(0);
        }
    }
}
