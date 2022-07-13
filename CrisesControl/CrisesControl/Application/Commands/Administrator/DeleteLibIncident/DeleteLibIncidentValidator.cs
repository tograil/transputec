using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident
{
    public class DeleteLibIncidentValidator:AbstractValidator<DeleteLibIncidentRequest>
    {
        public DeleteLibIncidentValidator()
        {
            RuleFor(x => x.LibIncidentId).GreaterThan(0);
        }
    }
}
