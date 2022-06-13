using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.TempRegister
{
    public class TempRegisterValidator:AbstractValidator<TempRegisterRequest>
    {
        public TempRegisterValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.Email));
        }
    }
}
