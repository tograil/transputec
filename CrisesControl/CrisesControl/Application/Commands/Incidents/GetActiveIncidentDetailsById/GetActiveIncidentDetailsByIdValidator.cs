using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentDetailsById
{
    public class GetActiveIncidentDetailsByIdValidator:AbstractValidator<GetActiveIncidentDetailsByIdRequest>
    {
        public GetActiveIncidentDetailsByIdValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
