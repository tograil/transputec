using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class DepartmentQuery : IDepartmentQuery
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public DepartmentQuery(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper =  mapper;
        }

        public async Task<GetDepartmentsResponse> GetDepartments(GetDepartmentsRequest request)
        {
            var departments = await _departmentRepository.GetAllDepartments(request.CompanyId);
            List<GetDepartmentResponse> response = _mapper.Map<List<Department>, List<GetDepartmentResponse>>(departments.ToList());
            var result = new GetDepartmentsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetDepartmentResponse> GetDepartment(GetDepartmentRequest request)
        {
            var department = await _departmentRepository.GetDepartment(request.CompanyId, request.DepartmentId);
            GetDepartmentResponse response = _mapper.Map<Department, GetDepartmentResponse>(department);

            return response;
        }
    }
}
