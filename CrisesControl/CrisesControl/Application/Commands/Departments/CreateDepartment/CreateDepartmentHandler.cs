using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentHandler: IRequestHandler<CreateDepartmentRequest, CreateDepartmentResponse>
    {
        private readonly CreateDepartmentValidator _departmentValidator;
        private readonly IDepartmentRepository _departmentRepository;

        public CreateDepartmentHandler(CreateDepartmentValidator departmentValidator, IDepartmentRepository departmentService)
        {
            _departmentValidator = departmentValidator;
            _departmentRepository = departmentService;
        }

        public async Task<CreateDepartmentResponse> Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateDepartmentRequest));

            var sample = new Department();
            var departments = await _departmentRepository.CreateDepartment(sample, cancellationToken);

            return new CreateDepartmentResponse();
        }
    }
}
