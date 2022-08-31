using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Sop.DeleteSOP
{
    public class DeleteSOPValidator:AbstractValidator<DeleteSOPRequest>
    {
        public DeleteSOPValidator()
        {
            RuleFor(x => x.SOPHeaderID).GreaterThan(0);
        }
    }
}
