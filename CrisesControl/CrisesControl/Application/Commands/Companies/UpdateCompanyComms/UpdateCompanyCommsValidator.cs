using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms
{
    public class UpdateCompanyCommsValidator:AbstractValidator<UpdateCompanyCommsRequest>
    {
        public UpdateCompanyCommsValidator()
        {
            RuleFor(x => x.MethodId).NotEmpty();
        }
    }
}
