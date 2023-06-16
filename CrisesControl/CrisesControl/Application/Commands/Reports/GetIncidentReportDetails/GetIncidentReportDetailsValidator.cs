using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReportDetails
{
    public class GetIncidentReportDetailsValidator:AbstractValidator<GetIncidentReportDetailsRequest>
    {
        public GetIncidentReportDetailsValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
