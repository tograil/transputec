using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.CheckDepartment
{
    public class CheckDepartmentValidator:AbstractValidator<CheckDepartmentRequest>
    {
        public CheckDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}
