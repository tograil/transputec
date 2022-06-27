using FluentValidation;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyParameterByName
{
    public class GetCompanyParameterByNameValidator:AbstractValidator<GetCompanyParameterByNameRequest>
    {
        public GetCompanyParameterByNameValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.CustomerId));
            RuleFor(x => string.IsNullOrEmpty(x.ParamName));
        }
    }
}
