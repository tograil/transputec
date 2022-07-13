using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncident
{
    public class GetLibIncidentValidator:AbstractValidator<GetLibIncidentRequest>
    {
        public GetLibIncidentValidator()
        {
            RuleFor(l => l.LibIncidentId).GreaterThan(0);
        }
    }
}
