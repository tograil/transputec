using FluentValidation;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters {
    public class GetCompanyParametersValidator : AbstractValidator<GetAllCompanyParametersRequest> {
        public GetCompanyParametersValidator() {
            RuleFor(x => x.CompanyId).GreaterThan(0).NotEmpty().ToString();
        }
    }
}
