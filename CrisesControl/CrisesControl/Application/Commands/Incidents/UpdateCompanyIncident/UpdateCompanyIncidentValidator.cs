using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncident
{
    public class UpdateCompanyIncidentValidator:AbstractValidator<UpdateCompanyIncidentRequest>
    {
        public UpdateCompanyIncidentValidator()
        {
            RuleFor(x=>x.IncidentId).GreaterThan(0);
            RuleFor(x =>string.IsNullOrEmpty( x.Name));
        }
    }
}
