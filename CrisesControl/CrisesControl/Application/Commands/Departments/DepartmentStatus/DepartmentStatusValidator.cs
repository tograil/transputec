using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.DepartmentStatus
{
    public class DepartmentStatusValidator:AbstractValidator<DepartmentStatusRequest>
    {
        public DepartmentStatusValidator()
        {
            RuleFor(x => x.OutUserCompanyId).GreaterThan(0);
        }
    }
}
