using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.BulkAction
{
    public class BulkActionValidator : AbstractValidator<BulkActionRequest>
    {
        public BulkActionValidator()
        {
            RuleFor(x => x.Action)
                .NotEmpty();
            RuleFor(x => x.ObjMapId)
                .NotEmpty();
        }
    }
}
