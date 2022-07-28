using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails
{
    public class GetIndidentReportDetailsValidator:AbstractValidator<GetIndidentReportDetailsRequest>
    {
        public GetIndidentReportDetailsValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
