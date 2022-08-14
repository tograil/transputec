using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentRecipientEntity
{
    public class GetIncidentRecipientEntityValidator:AbstractValidator<GetIncidentRecipientEntityRequest>
    {
        public GetIncidentRecipientEntityValidator()
        {
            RuleFor(x => x.ActiveIncidentID).GreaterThan(0);
            RuleFor(x => x.EntityID).GreaterThan(0);
           
        }
    }
}
