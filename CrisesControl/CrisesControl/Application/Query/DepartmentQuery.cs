using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.CheckDepartment;
using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.DeleteDepartment;
using CrisesControl.Api.Application.Commands.Departments.DepartmentStatus;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.Commands.Departments.SegregationLinks;
using CrisesControl.Api.Application.Commands.Departments.UpdateDepartment;
using CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups;
using CrisesControl.SharedKernel.Utils;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class DepartmentQuery : IDepartmentQuery
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
       
        private readonly ICurrentUser _currentUser;
        public DepartmentQuery(IDepartmentRepository departmentRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _departmentRepository = departmentRepository;
            _mapper =  mapper;
           
            _currentUser = currentUser;
        }

        public async Task<GetDepartmentsResponse> GetDepartments(GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentsRequest));

            var departments = await _departmentRepository.GetAllDepartments(request.CompanyId);
            var result = _mapper.Map<List<Department>>(departments.ToList());
            var response = new GetDepartmentsResponse();
            response.Data = result;
            return response;
        }

        public async Task<GetDepartmentResponse> GetDepartment(GetDepartmentRequest request, CancellationToken cancellationToken)
        {


            var department = await _departmentRepository.GetDepartment(request.CompanyId, request.DepartmentId);
            var result  = _mapper.Map<GetDepartmentResponse>(department);
            
            return result;
        }

        public async Task<CheckDepartmentResponse> CheckDepartment(CheckDepartmentRequest request)
        {
            try
            {
                var departments =await _departmentRepository.CheckForExistance(request.DepartmentId);
                var result = _mapper.Map<bool>(departments);
                var response = new CheckDepartmentResponse();
                if (result) { 
                    response.IsDuplicated=result;
                    response.Message = "Duplicate Department.";
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
                var groupLinks = await  _departmentRepository.SegregationLinks(request.TargetID,request.MemberShipType, request.LinkType, _currentUser.UserId,request.OutUserCompanyId);
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
                var departments =await _departmentRepository.UpdateSegregationLink(request.SourceId, request.TargetId,request.LinkType,_currentUser.CompanyId);
                var result = _mapper.Map<bool>(departments);
                var response = new UpdateSegregationLinkResponse();
                if (result)
                {
                    response.updatedSeg = result;
                    response.Message = "Updated.";
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

        public async Task<CreateDepartmentResponse> CreateDepartment(CreateDepartmentRequest request,CancellationToken cancellationToken)
        {
            try
            {
                Department department = new Department()
                {
                    CompanyId = request.CompanyId,
                    CreatedBy = _currentUser.UserId,
                    CreatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                    DepartmentName = request.DepartmentName,
                    Status = 1,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn= DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),



                };
                if (!_departmentRepository.CheckDuplicate(department))
                {
                    var depart = await _departmentRepository.CreateDepartment(department, cancellationToken);
                    var result = _mapper.Map<int>(depart);
                    var response = new CreateDepartmentResponse();
                    if (result > 0)
                    {
                        response.DepartmentId = result;

                    }
                    else
                    {
                        response.DepartmentId = result;

                    }
                    return response;
                }
                return new CreateDepartmentResponse();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateDepartmentResponse> UpdateDepartment(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Department department = new Department()
                {
                    DepartmentId=request.DepartmentId,
                    CompanyId = request.CompanyId,
                    DepartmentName = request.DepartmentName,
                    Status = 1,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                };
                if (!_departmentRepository.CheckDuplicate(department))
                {
                    var depart = await _departmentRepository.UpdateDepartment(department, cancellationToken);
                    var result = _mapper.Map<int>(depart);
                    var response = new UpdateDepartmentResponse();
                    if (result > 0)
                    {
                        response.DepartmentId = result;

                    }
                    else
                    {
                        response.DepartmentId = result;

                    }
                    return response;
                }
                return new UpdateDepartmentResponse();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<DeleteDepartmentResponse> DeleteDepartment(DeleteDepartmentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                    var departmentId = await _departmentRepository.DeleteDepartment(request.DepartmentId, cancellationToken);
                    var result = _mapper.Map<int>(departmentId);
                    var response = new DeleteDepartmentResponse();
                    if (result > 0)
                    {
                        response.DepartmentId = result;

                    }
                    else
                    {
                        response.DepartmentId = result;

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
