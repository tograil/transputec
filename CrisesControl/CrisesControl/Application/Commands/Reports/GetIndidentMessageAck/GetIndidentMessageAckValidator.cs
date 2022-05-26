using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckValidator:  AbstractValidator<GetIndidentMessageAckRequest>
    {
        public GetIndidentMessageAckValidator()
        {
            //RuleFor(x => x.RecordStart).GreaterThan(0).ToString();            
            //RuleFor(x => x.RecordLength).GreaterThan(0).ToString();

        }
    }
}
