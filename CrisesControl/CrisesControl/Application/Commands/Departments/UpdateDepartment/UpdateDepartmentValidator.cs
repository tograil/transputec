using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateDepartment
{
    public class UpdateDepartmentValidator: AbstractValidator<UpdateDepartmentRequest>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
