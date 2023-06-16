//using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.CheckDepartment;
using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.DeleteDepartment;
using CrisesControl.Api.Application.Commands.Departments.DepartmentStatus;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.Commands.Departments.SegregationLinks;
using CrisesControl.Api.Application.Commands.Departments.UpdateDepartment;
using CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink;

namespace CrisesControl.Api.Application.Query
{
    public interface IDepartmentQuery
    {
        public Task<GetDepartmentsResponse> GetDepartments(GetDepartmentsRequest request, CancellationToken cancellationToken);
        public Task<GetDepartmentResponse> GetDepartment(GetDepartmentRequest request, CancellationToken cancellationToken);
        Task<CreateDepartmentResponse> CreateDepartment(CreateDepartmentRequest request, CancellationToken cancellationToken);
        Task<UpdateDepartmentResponse> UpdateDepartment(UpdateDepartmentRequest request, CancellationToken cancellationToken);
        Task<DeleteDepartmentResponse> DeleteDepartment(DeleteDepartmentRequest request, CancellationToken cancellationToken);
        Task<CheckDepartmentResponse> CheckDepartment(CheckDepartmentRequest request);
        Task<DepartmentStatusResponse> DepartmentStatus(DepartmentStatusRequest request);
        Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request);
        Task<UpdateSegregationLinkResponse> UpdateSegregationLink(UpdateSegregationLinkRequest request);

    }
}
