using CrisesControl.Core.Departments;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Groups = CrisesControl.Core.Groups.Group;

namespace CrisesControl.Infrastructure.Repositories
{
    public class GroupRepository: IGroupRepository
    {
        private readonly CrisesControlContext _context;
        private int UserID;
        private int CompanyID;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupRepository> _logger;

        public GroupRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor, ILogger<GroupRepository> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
        }

        public async Task<int> CreateGroup(Groups group, CancellationToken token)
        {
            await _context.AddAsync(group, token);

            await _context.SaveChangesAsync(token);

            return group.GroupId;
        }

        public async Task<int> DeleteGroup(int groupId, CancellationToken token)
        {
            await _context.AddAsync(groupId);

            await _context.SaveChangesAsync(token);

            return groupId;
        }

        public async Task<IEnumerable<Groups>> GetAllGroups(int companyId)
        {
            return await _context.Set<Groups>().AsNoTracking().Where(t => t.CompanyId == companyId).ToListAsync();
        }


        public async Task<Groups> GetGroup(int companyId, int groupId)
        {
            return await _context.Set<Groups>().AsNoTracking().Where(t => t.CompanyId == companyId && t.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateGroup(Groups group, CancellationToken token)
        {
            var result = _context.Set<Groups>().Where(t => t.GroupId == group.GroupId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.GroupName = group.GroupName;
                result.Status = group.Status;
                result.UpdatedOn = group.UpdatedOn;
                result.UpdatedBy = group.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.GroupId;
            }
        }

        public bool CheckDuplicate(Groups group)
        {
            return _context.Set<Groups>().Where(t=>t.GroupName.Equals(group.GroupName)).Any();
        }

        public bool CheckForExistance(int groupId)
        {
            return _context.Set<Groups>().Where(t=>t.GroupId.Equals(groupId)).Any();
        }
        public async Task<List<GroupLink>> SegregationLinks(int TargetID, MemberShipType MemberShipType,string LinkType)
        {
            try
            {
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                var pGroupID = new SqlParameter("@GroupID", TargetID);
                var pMemberShipType = new SqlParameter("@MemberShipType", MemberShipType.ToMemString());
                var pLinkType = new SqlParameter("@LinkType", LinkType);
                var pUserID = new SqlParameter("@UserID", UserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await _context.Set<GroupLink>().FromSqlRaw("exec Pro_Get_Group_Links @GroupID, @LinkType, @MemberShipType, @UserID, @CompanyID",
                    pGroupID, pLinkType, pMemberShipType, pUserID, pCompanyID).ToListAsync();
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DuplicateGroup(string strGroupName, int intcompanyid, int intGroupId)
        {
            try
            {
                if (intGroupId == 0)
                {
                    var Dept = await _context.Set<Groups>()
                                .Where(Depval=> Depval.GroupName == strGroupName && Depval.CompanyId == intcompanyid
                                ).FirstOrDefaultAsync();

                    if (Dept != null)
                    {
                        return true;
                    }
                }
                else 
                {
                    var Dept = await _context.Set<Groups>()
                                .Where(Depval => Depval.GroupName == strGroupName && Depval.CompanyId == intcompanyid && (Depval.GroupId) != intGroupId
                                ).FirstOrDefaultAsync();

                    if (Dept != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }

        public async Task<bool> UpdateSegregationLink(int sourceId, int targetId, string action, string linkType)
        {
            try
            {
                if (linkType == "DEPARTMENT")
                {
                    var item = await (from I in _context.Set<SegGroupDepartmentLink>() where I.GroupId == sourceId && I.DepartmentId == targetId select I).FirstOrDefaultAsync();
                    if (action.ToUpper() == "ADD" && item == null)
                    {
                        await CreateSegregtionLink(sourceId, targetId, linkType, CompanyID);
                    }
                    else if (action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Set<SegGroupDepartmentLink>().Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "GROUP")
                {
                    var item = await (from I in _context.Set<SegGroupDepartmentLink>() where I.DepartmentId == sourceId && I.GroupId == targetId select I).FirstOrDefaultAsync();
                    if (action.ToUpper() == "ADD" && item == null)
                    {
                        await CreateSegregtionLink(sourceId, targetId, linkType, CompanyID);
                    }
                    else if (action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Set<SegGroupDepartmentLink>().Remove(item);
                        _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "INCIDENT")
                {
                    var item = await (from I in _context.Set<SegGroupIncidentLink>() where I.GroupId == sourceId && I.IncidentId == targetId select I).FirstOrDefaultAsync();
                    if (action.ToUpper() == "ADD" && item == null)
                    {
                        await CreateSegregtionLink(sourceId, targetId, linkType, CompanyID);
                    }
                    else if (action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Set<SegGroupIncidentLink>().Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "LOCATION")
                {
                    var item = await (from I in _context.Set<SegGroupLocationLink>() where I.GroupId == sourceId && I.LocationId == targetId select I).FirstOrDefaultAsync();
                    if (action.ToUpper() == "ADD" && item == null)
                    {
                        await CreateSegregtionLink(sourceId, targetId, linkType, CompanyID);
                    }
                    else if (action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Set<SegGroupLocationLink>().Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task CreateSegregtionLink(int sourceId, int targetId, string linkType, int companyId)
        {
            try
            {
                if (linkType == "DEPARTMENT")
                {
                    var targtetitem = _context.Set<Department>().Where(w => w.DepartmentId == targetId && w.CompanyId == companyId).FirstOrDefault();
                    if (targtetitem != null)
                    {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink()
                        {
                            CompanyId = companyId,
                            DepartmentId = targetId,
                            GroupId = sourceId
                        };
                        _context.Set<SegGroupDepartmentLink>().Add(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "GROUP")
                {
                    var targtetitem = await _context.Set<Core.Models.Group>().Where(w => w.GroupId == targetId && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink()
                        {
                            CompanyId = companyId,
                            DepartmentId = sourceId,
                            GroupId = targetId
                        };
                        _context.Set<SegGroupDepartmentLink>().Add(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "INCIDENT")
                {
                    var targtetitem = await _context.Set<Incident>().Where(w => w.IncidentId == targetId && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegGroupIncidentLink SGL = new SegGroupIncidentLink()
                        {
                            CompanyId = companyId,
                            IncidentId = targetId,
                            GroupId = sourceId
                        };
                        _context.Set<SegGroupIncidentLink>().Add(SGL);
                        _context.SaveChangesAsync();
                    }
                }
                else if (linkType == "LOCATION")
                {
                    var targtetitem = _context.Set<Location>().Where(w => w.LocationId == targetId && w.CompanyId == companyId).FirstOrDefault();
                    if (targtetitem != null)
                    {
                        SegGroupLocationLink SGL = new SegGroupLocationLink()
                        {
                            CompanyId = companyId,
                            LocationId = targetId,
                            GroupId = sourceId
                        };
                        _context.Set<SegGroupLocationLink>().Add(SGL);
                        _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
