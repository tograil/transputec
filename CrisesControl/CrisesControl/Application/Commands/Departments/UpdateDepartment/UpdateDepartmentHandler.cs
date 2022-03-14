using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateDepartment
{
    public class UpdateDepartmentHandler: IRequestHandler<UpdateDepartmentRequest, UpdateDepartmentResponse>
    {
        private readonly UpdateDepartmentValidator _departmentValidator;
        private readonly IDepartmentRepository _departmentRepository;

        public UpdateDepartmentHandler(UpdateDepartmentValidator departmentValidator, IDepartmentRepository departmentService)
        {
            _departmentValidator = departmentValidator;
            _departmentRepository = departmentService;
        }

        public async Task<UpdateDepartmentResponse> Handle(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateDepartmentRequest));

            var sample = new Department();
            var departmentId = await _departmentRepository.UpdateDepartment(sample, cancellationToken);
            var result = new UpdateDepartmentResponse();
            result.DepartmentId = departmentId;   
            return result;
        }
    }
}
