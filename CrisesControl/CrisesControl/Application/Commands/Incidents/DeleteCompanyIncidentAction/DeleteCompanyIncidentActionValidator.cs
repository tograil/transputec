using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncidentAction
{
    public class DeleteCompanyIncidentActionValidator:AbstractValidator<DeleteCompanyIncidentActionRequest>
    {
        public DeleteCompanyIncidentActionValidator()
        {
            RuleFor(x => x.IncidentActionId).GreaterThan(0);
            RuleFor(x => x.IncidentId).GreaterThan(0);
        }
    }
}
