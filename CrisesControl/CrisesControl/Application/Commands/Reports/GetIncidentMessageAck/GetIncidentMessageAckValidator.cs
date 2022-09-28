using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck
{
    public class GetIncidentMessageAckValidator:  AbstractValidator<GetIncidentMessageAckRequest>
    {
        public GetIncidentMessageAckValidator()
        {
            //RuleFor(x => x.RecordStart).GreaterThan(0).ToString();            
            //RuleFor(x => x.RecordLength).GreaterThan(0).ToString();

        }
    }
}
