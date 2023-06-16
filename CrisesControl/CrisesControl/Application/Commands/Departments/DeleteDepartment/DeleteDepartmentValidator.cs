using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.DeleteDepartment
{
    public class DeleteDepartmentValidator:AbstractValidator<DeleteDepartmentRequest>
    {
        public DeleteDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}
