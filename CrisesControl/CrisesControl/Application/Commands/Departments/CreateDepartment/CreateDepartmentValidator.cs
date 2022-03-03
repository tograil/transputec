using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentValidator: AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
