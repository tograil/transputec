using FluentValidation;

namespace CrisesControl.Core.CompanyAggregate.Handlers.TempRegister;

public class TempRegisterValidator : AbstractValidator<TempRegisterRequest>
{
    public TempRegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();
        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}