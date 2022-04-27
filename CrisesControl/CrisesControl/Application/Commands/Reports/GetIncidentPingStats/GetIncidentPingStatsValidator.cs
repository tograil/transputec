using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats {
    public class GetIncidentPingStatsValidator : AbstractValidator<GetIncidentPingStatsRequest> {
        public GetIncidentPingStatsValidator() {
            RuleFor(x => x.CompanyId).GreaterThan(0).ToString();
            RuleFor(x => x.NoOfMonth).GreaterThan(0).ToString();
        }
    }
}
