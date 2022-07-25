using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit
{
    public class GetTaskAuditValidator:AbstractValidator<GetTaskAuditRequest>
    {
        public GetTaskAuditValidator()
        {
            RuleFor(x => x.ActiveIncidentTaskID).GreaterThan(0);
        }
    }
}
