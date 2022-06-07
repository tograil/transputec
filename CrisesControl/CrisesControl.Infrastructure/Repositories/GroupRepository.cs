using CrisesControl.Core.Groups;
using CrisesControl.Core.Groups.Repositories;
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
        }

        public async Task<int> CreateGroup(Group group, CancellationToken token)
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

        public async Task<IEnumerable<Group>> GetAllGroups(int companyId)
        {
            return await _context.Set<Group>().AsNoTracking().Where(t => t.CompanyId == companyId).ToListAsync();
        }


        public async Task<Group> GetGroup(int companyId, int groupId)
        {
            return await _context.Set<Group>().AsNoTracking().Where(t => t.CompanyId == companyId && t.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateGroup(Group group, CancellationToken token)
        {
            var result = _context.Set<Group>().Where(t => t.GroupId == group.GroupId).FirstOrDefault();

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

        public bool CheckDuplicate(Group group)
        {
            return _context.Set<Group>().Where(t=>t.GroupName.Equals(group.GroupName)).Any();
        }

        public bool CheckForExistance(int groupId)
        {
            return _context.Set<Group>().Where(t=>t.GroupId.Equals(groupId)).Any();
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
    }
}
