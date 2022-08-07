using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CheckDepartment
{
    public class CheckDepartmentHandler : IRequestHandler<CheckDepartmentRequest, CheckDepartmentResponse>
    {
        private readonly IDepartmentQuery _departmentQuery;
        private readonly IMapper _mapper;
        public CheckDepartmentHandler(IMapper mapper, IDepartmentQuery departmentQuery)
        {
            this._departmentQuery = departmentQuery;
            this._mapper = mapper;
        }
        public async Task<CheckDepartmentResponse> Handle(CheckDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _departmentQuery.CheckDepartment(request);
            return result;
        }
    }
}
