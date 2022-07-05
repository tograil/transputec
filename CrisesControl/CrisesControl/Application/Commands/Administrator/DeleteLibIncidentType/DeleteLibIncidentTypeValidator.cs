using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType
{
    public class DeleteLibIncidentTypeValidator:AbstractValidator<DeleteLibIncidentTypeRequest>
    {
        public DeleteLibIncidentTypeValidator()
        {
            RuleFor(x => x.LibIncidentTypeId).GreaterThan(0);
        }
    }
}
