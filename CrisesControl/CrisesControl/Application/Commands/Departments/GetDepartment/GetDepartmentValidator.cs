using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentValidator: AbstractValidator<GetDepartmentRequest>
    {
        public GetDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
            RuleFor(x => x.DepartmentName)
                .NotNull();
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
