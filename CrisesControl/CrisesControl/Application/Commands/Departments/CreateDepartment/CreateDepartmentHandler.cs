using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentHandler: IRequestHandler<CreateDepartmentRequest, CreateDepartmentResponse>
    {
        private readonly CreateDepartmentValidator _departmentValidator;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public CreateDepartmentHandler(CreateDepartmentValidator departmentValidator, IDepartmentRepository departmentService, IMapper mapper)
        {
            _departmentValidator = departmentValidator;
            _departmentRepository = departmentService;
            _mapper = mapper;
        }

        public async Task<CreateDepartmentResponse> Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateDepartmentRequest));

            Department value = _mapper.Map<CreateDepartmentRequest, Department>(request);
            if (CheckDuplicate(value))
            {
                var departmentId = await _departmentRepository.CreateDepartment(value, cancellationToken);
                var result = new CreateDepartmentResponse();
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
