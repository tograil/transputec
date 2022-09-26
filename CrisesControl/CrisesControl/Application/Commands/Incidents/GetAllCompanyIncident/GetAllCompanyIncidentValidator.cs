using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident
{
    public class GetAllCompanyIncidentValidator:AbstractValidator<GetAllCompanyIncidentRequest>
    {
        public GetAllCompanyIncidentValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
