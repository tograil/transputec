using Ardalis.GuardClauses;
using AutoMapper;
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
        private readonly IMapper _mappper;

        public UpdateDepartmentHandler(UpdateDepartmentValidator departmentValidator, IDepartmentRepository departmentService, IMapper mapper)
        {
            _departmentValidator = departmentValidator;
            _departmentRepository = departmentService;
            _mappper = mapper;
        }

        public async Task<UpdateDepartmentResponse> Handle(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateDepartmentRequest));

            Department value = _mappper.Map<UpdateDepartmentRequest,Department>(request);
            if (CheckDuplicate(value))
            {
                var departmentId = await _departmentRepository.UpdateDepartment(value, cancellationToken);
                var result = new UpdateDepartmentResponse();
                result.DepartmentId = departmentId;   
                return result;
            }
            return null;
        }

        private bool CheckDuplicate(Department department)
        {
            return _departmentRepository.CheckDuplicate(department);
        }
    }
}
