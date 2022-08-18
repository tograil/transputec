using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncident
{
    public class DeleteCompanyIncidentValidator:AbstractValidator<DeleteCompanyIncidentRequest>
    {
        public DeleteCompanyIncidentValidator()
        {
            RuleFor(x => x.IncidentId).GreaterThan(0);
        }
    }
}
