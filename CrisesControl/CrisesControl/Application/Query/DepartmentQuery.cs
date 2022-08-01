using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.CheckDepartment;
using CrisesControl.Api.Application.Commands.Departments.DepartmentStatus;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.Commands.Departments.SegregationLinks;
using CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class DepartmentQuery : IDepartmentQuery
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly GetDepartmentValidator _departmentValidator;
        private readonly ICurrentUser _currentUser;
        public DepartmentQuery(IDepartmentRepository departmentRepository, IMapper mapper, GetDepartmentValidator departmentValidator, ICurrentUser currentUser)
        {
            _departmentRepository = departmentRepository;
            _mapper =  mapper;
            _departmentValidator = departmentValidator;
            _currentUser = currentUser;
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

        public async Task<CheckDepartmentResponse> CheckDepartment(CheckDepartmentRequest request)
        {
            try
            {
                var departments = _departmentRepository.CheckForExistance(request.DepartmentId);
                var result = _mapper.Map<bool>(departments);
                var response = new CheckDepartmentResponse();
                if (result) { 
                    response.IsDuplicated=result;
                    response.Message = "Duplicate Deprtment.";
                }
                else
                {
                    response.IsDuplicated = result;
                    response.Message = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DepartmentStatusResponse> DepartmentStatus(DepartmentStatusRequest request)
        {
            try
            {
                var department = await _departmentRepository.DepartmentStatus(request.OutUserCompanyId);
                var result = _mapper.Map<int>(department);
                var response = new DepartmentStatusResponse();
                if (result>0)
                {
                    response.result = result;
                   
                }
                else
                {
                    response.result = result;
                    
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request)
        {
            try
            {
                var groupLinks = _departmentRepository.SegregationLinks(request.TargetID,request.MemberShipType, request.LinkType, _currentUser.UserId,request.OutUserCompanyId);
                var result = _mapper.Map<List<GroupLink>>(groupLinks);
                var response = new SegregationLinksResponse();
                if (result!=null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateSegregationLinkResponse> UpdateSegregationLink(UpdateSegregationLinkRequest request)
        {
            try
            {
                var departments = _departmentRepository.UpdateSegregationLink(request.SourceId, request.TargetId, request.Action,request.LinkType,_currentUser.CompanyId);
                var result = _mapper.Map<bool>(departments);
                var response = new UpdateSegregationLinkResponse();
                if (result)
                {
                    response.updatedSeg = result;
                    response.Message = "Duplicate Deprtment.";
                }
                else
                {
                    response.updatedSeg = false;
                    response.Message = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
