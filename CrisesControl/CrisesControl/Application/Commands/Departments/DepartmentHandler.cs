using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Departments.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments
{
    
    public class DepartmentHandler : IRequestHandler<DepartmentsRequest, DepartmentsResponse>
    {
        private readonly IDepartmentRepository _departmentQuery;   
        public DepartmentHandler(IDepartmentRepository departmentQuery)
        {
            this._departmentQuery = departmentQuery;
        }
        public Task<DepartmentsResponse> Handle(DepartmentsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
