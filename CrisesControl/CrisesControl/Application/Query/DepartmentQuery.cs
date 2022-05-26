using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class DepartmentQuery : IDepartmentQuery
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly GetDepartmentValidator _departmentValidator;
        public DepartmentQuery(IDepartmentRepository departmentRepository, IMapper mapper, GetDepartmentValidator departmentValidator)
        {
            _departmentRepository = departmentRepository;
            _mapper =  mapper;
            _departmentValidator = departmentValidator;
        }

        public async Task<GetDepartmentsResponse> GetDepartments(GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentsRequest));

            var departments = await _departmentRepository.GetAllDepartments(request.CompanyId);
            List<GetDepartmentResponse> response = _mapper.Map<List<Department>, List<GetDepartmentResponse>>(departments.ToList());
            var result = new GetDepartmentsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetDepartmentResponse> GetDepartment(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentRequest));

            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);

            var department = await _departmentRepository.GetDepartment(request.CompanyId, request.DepartmentId);
            GetDepartmentResponse response = _mapper.Map<Department, GetDepartmentResponse>(department);

            return response;
        }
    }
}
