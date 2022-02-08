using FluentValidation;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany;

public class GetCompanyValidator : AbstractValidator<GetCompanyRequest>
{
    public GetCompanyValidator()
    {
        RuleFor(x => x.CompanyId)
            .GreaterThan(0);
    }
}