using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentStats
{
    public class GetIncidentStatsValidator:AbstractValidator<GetIncidentStatsRequest>
    {
        public GetIncidentStatsValidator()
        {
            RuleFor(x=>x.IncidentActivationId).GreaterThan(0);
        }
    }
}
