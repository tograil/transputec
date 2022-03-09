using Ardalis.GuardClauses;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentHandler: IRequestHandler<GetDepartmentRequest, GetDepartmentResponse>
    {
        private readonly GetDepartmentValidator _departmentValidator;
        private readonly IDepartmentRepository _departmentService;

        public GetDepartmentHandler(GetDepartmentValidator departmentValidator, IDepartmentRepository departmentService)
        {
            _departmentValidator = departmentValidator;
            _departmentService = departmentService;
        }

        public async Task<GetDepartmentResponse> Handle(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentRequest));
            
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _departmentService.GetAllDepartments();

            return new GetDepartmentResponse();
        }
    }
}
