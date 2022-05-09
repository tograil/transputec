using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompany {
    public class GetCompanyValidator : AbstractValidator<GetCompanyRequest> {
        public GetCompanyValidator() {
            RuleFor(x => x.CompanyId)
                .NotNull();
        }
    }
}
