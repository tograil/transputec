using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageNoAck
{
    public class GetIncidentMessageNoAckValidator:AbstractValidator<GetIncidentMessageNoAckRequest> 
    {
         public GetIncidentMessageNoAckValidator()
        {
            RuleFor(x => x.RecordLength).GreaterThan(0).ToString();
            RuleFor(x => x.IncidentActivationId).GreaterThan(0).ToString();
            RuleFor(x => x.RecordStart).GreaterThan(0).ToString();
            RuleFor(x=> string.IsNullOrEmpty(x.SearchString) || !string.IsNullOrEmpty(x.SearchString));
            RuleFor(x => string.IsNullOrEmpty(x.UniqueKey) || !string.IsNullOrEmpty(x.UniqueKey));
        }
    }
}
