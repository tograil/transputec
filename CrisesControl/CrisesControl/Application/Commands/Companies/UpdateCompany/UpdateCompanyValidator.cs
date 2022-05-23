using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompany
{
    public class UpdateCompanyValidator:AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => x.TimeZone).GreaterThan(0);           
            RuleFor(x => string.IsNullOrEmpty(x.SwitchBoardPhone));
            RuleFor(x => string.IsNullOrEmpty(x.AddressLine1));            
            RuleFor(x => string.IsNullOrEmpty(x.AddressLine2));
            RuleFor(x => string.IsNullOrEmpty(x.CountryCode));
            RuleFor(x => string.IsNullOrEmpty(x.Postcode));
            RuleFor(x => string.IsNullOrEmpty(x.City));
        }
       
    }
}
