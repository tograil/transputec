using AutoMapper;
using CrisesControl.Api.Application.Commands.Security.AddSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects;
using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Models;
using CrisesControl.Core.Security;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class SecurityQuery : ISecurityQuery
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SecurityQuery> _logger;
        private readonly ICurrentUser _currentUser;
        public SecurityQuery(ISecurityRepository securityRepository, IMapper _mapper, ILogger<SecurityQuery> logger, ICurrentUser currentUser)
        {
            this._mapper = _mapper;
            this._logger = logger;
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }

        public async Task<AddSecurityGroupResponse> AddSecurityGroup(AddSecurityGroupRequest request)
        {
            try
            {
              var response=  new AddSecurityGroupResponse();
                SecurityGroup secGroup = new SecurityGroup()
                {
                    CompanyId = _currentUser.CompanyId,
                    Name = request.GroupName,
                    Description = request.GroupDescription,
                    Status = request.Status,
                    UserRole = request.UserRole,
                    CreatedBy = _currentUser.UserId,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                };
                var SercirityId = await _securityRepository.AddSecurityGroup(secGroup);


                if (request.GroupSecurityObjects != null)
                {
                    foreach (int SecObj in request.GroupSecurityObjects)
                    {
                        GroupSecuityObject SecItem = new GroupSecuityObject()
                        {
                            SecurityGroupId = secGroup.SecurityGroupId,
                            SecurityObjectId = Convert.ToInt32(SecObj)
                        };
                        await _securityRepository.AddGroupSecuityObject(SecItem);
                    }
                }

                response.result =secGroup.SecurityGroupId;
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeleteSecurityGroupResponse> DeleteSecurityGroup(DeleteSecurityGroupRequest request)
        {
            try
            {
                var response = new DeleteSecurityGroupResponse();
                var checkGroup = await _securityRepository.CheckMenuAccessAssociation(request.SecurityGroupId, _currentUser.CompanyId);
                if (checkGroup)
                {
                    response.result = 0;
                    response.Message = "Security group attached to user, cannot be deleted.";
                }

                var GroupData = await _securityRepository.GetSecurityGroup(request.SecurityGroupId, _currentUser.CompanyId);
                if (GroupData != null)
                {
                    GroupData.Status = 3;
                    GroupData.Name = "DEL_" + GroupData.Name;
                    GroupData.UpdatedBy = _currentUser.UserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone, DateTime.Now);
                   int updateSecurityId= await _securityRepository.UpdateSecurityGroup(GroupData);

                    //var delUserSecurityGroup = (from USG in db.UserSecurityGroup where USG.SecurityGroupId == SecurityGroupId select USG).ToList();
                    //db.UserSecurityGroup.RemoveRange(delUserSecurityGroup);
                    //db.SaveChanges(CurrentUserId, CompanyID);
                    response.result = updateSecurityId;
                    response.Message = "Deleted";
                }
                else
                {
                    response.result = 0;
                    response.Message = "No data Found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAllSecurityObjectsResponse> GetAllSecurityObjects(GetAllSecurityObjectsRequest request)
        {
            try
            {

                var sopSection = await _securityRepository.GetAllSecurityObjects(_currentUser.CompanyId);
                var result = _mapper.Map<List<SecurityAllObjects>>(sopSection);
                var response = new GetAllSecurityObjectsResponse();

                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No Data Found";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanySecurityGroupResponse> GetCompanySecurityGroup(GetCompanySecurityGroupRequest request)
        {
            var securities = await _securityRepository.GetCompanySecurityGroup(request.CompanyID);
            //List<GetCompanySecurityGroupResponse> response = _mapper.Map<List<CompanySecurityGroup>, List<GetCompanySecurityGroupResponse>>(securities.ToList());
            var response = _mapper.Map<List<CompanySecurityGroup>>(securities);
            var result = new GetCompanySecurityGroupResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetSecurityGroupResponse> GetSecurityGroup(GetSecurityGroupRequest request)
        {
            try
            {
                var security = await _securityRepository.GetSecurityGroup(request.SecurityGroupId, _currentUser.CompanyId);
                var result = _mapper.Map<SecurityGroup>(security);
                var response = new GetSecurityGroupResponse();

                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateSecurityGroupResponse> UpdateSecurityGroup(UpdateSecurityGroupRequest request)
        {
            try
            {
                var GroupData = await _securityRepository.GetSecurityGroup(request.SecurityGroupId, _currentUser.CompanyId);
                var response = new UpdateSecurityGroupResponse();
                if (GroupData != null)
                {
                    GroupData.Name = request.GroupName;
                    GroupData.Description = request.GroupDescription;
                    GroupData.UserRole = request.UserRole;
                    GroupData.Status = request.Status;
                    GroupData.UpdatedBy = _currentUser.UserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone, System.DateTime.Now);
                };
                var groupId = await _securityRepository.UpdateSecurityGroup(GroupData);
                response.result = groupId;

                var RegGrpDel = await _securityRepository.GetGroupSecuityObject(request.SecurityGroupId);

                List<int[]> GSOList = new List<int[]>();
                if (request.GroupSecurityObjects != null)
                {
                    if (request.GroupSecurityObjects.Length > 0)
                    {
                        foreach (int SecObj in request.GroupSecurityObjects)
                        {
                            var ISExist = RegGrpDel.FirstOrDefault(s => s.SecurityGroupId == request.SecurityGroupId && s.SecurityObjectId == SecObj);
                            if (ISExist == null)
                            {
                                GroupSecuityObject SecItem = new GroupSecuityObject()
                                {
                                    SecurityGroupId = request.SecurityGroupId,
                                    SecurityObjectId = Convert.ToInt32(SecObj)
                                };
                                await _securityRepository.AddGroupSecuityObject(SecItem);
                            }
                            else
                            {
                                int[] Arr = new int[2];
                                Arr[0] = ISExist.SecurityGroupId;
                                Arr[1] = ISExist.SecurityObjectId;
                                GSOList.Add(Arr);
                            }
                            await _securityRepository.UpdateGroupSecuityObject(ISExist);
                        }

                        foreach (var Ditem in RegGrpDel)
                        {
                            bool ISDEL = GSOList.Any(s => s[0] == Ditem.SecurityGroupId && s[1] == Ditem.SecurityObjectId);
                            if (ISDEL == false)
                            {

                                await _securityRepository.DeleteGroupSecuityObject(Ditem);
                            }
                        }
                    }
                }
                if (request.Status == 0)
                {
                    var delUserSecurityGroup = await _securityRepository.GetUserSecurityGroup(request.SecurityGroupId);
                    await _securityRepository.DeleteUserSecurityGroup(delUserSecurityGroup);
                }
                response.result = request.SecurityGroupId;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
