using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsValidator: AbstractValidator<GetDepartmentsRequest>
    {
        public GetDepartmentsValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
