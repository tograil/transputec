using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckValidator:  AbstractValidator<GetIndidentMessageAckRequest>
    {
        public GetIndidentMessageAckValidator()
        {
            RuleFor(x => x.RecordStart).GreaterThan(0).ToString();            
            RuleFor(x => x.RecordLength).GreaterThan(0).ToString();
            RuleFor(x => x.Source == "WEB" || string.IsNullOrEmpty(x.Source));
            RuleFor(x => x.OrderBy == "user_id" || string.IsNullOrEmpty(x.OrderBy));
            RuleFor(x => x.Filters == "" || string.IsNullOrEmpty(x.Filters));

        }
    }
}
