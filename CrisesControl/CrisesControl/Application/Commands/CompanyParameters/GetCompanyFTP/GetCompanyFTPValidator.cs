using FluentValidation;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP
{
    public class GetCompanyFTPValidator: AbstractValidator<GetCompanyFTPRequest>
    {
        public GetCompanyFTPValidator()
        {

            RuleFor(x => x.CompanyID)
                .GreaterThan(0);
            
        }
    }
}
