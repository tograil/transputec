using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList
{
    public class GetTaskUserListValidator:AbstractValidator<GetTaskUserListRequest>
    {
        public GetTaskUserListValidator()
        {
            RuleFor(x => x.ActiveIncidentTaskID).GreaterThan(0);
            RuleFor(x => x.OutLoginCompanyId).GreaterThan(0);
            RuleFor(x => x.OutLoginUserId).GreaterThan(0);
        }
    }
}
