using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Department = CrisesControl.Core.Departments.Department;
using Group = CrisesControl.Core.Groups.Group;

namespace CrisesControl.Infrastructure.Repositories {
    public class DepartmentRepository : IDepartmentRepository {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int currentUserId;
        private int currentCompanyId;

        public DepartmentRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

            currentUserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            currentCompanyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        }

        public async Task<int> CreateDepartment(Department department, CancellationToken token) {
            await _context.AddAsync(department, token);

            await _context.SaveChangesAsync(token);

            return department.DepartmentId;
        }

        public async Task<int> DeleteDepartment(int departmentId, CancellationToken token) {
            var Department = await CheckForExistance(departmentId);
            if (Department) {
                var department = await _context.Set<Department>().Where(d => d.DepartmentId == departmentId).FirstOrDefaultAsync();
                _context.Remove(department);

                await _context.SaveChangesAsync(token);

                return departmentId;
            }
            return 0;
        }

        public async Task<List<DepartmentDetail>> GetAllDepartments(int companyId, bool filterVirtual = false) {

            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", currentUserId);
            var pFilterVirtual = new SqlParameter("@FilterVirtual", filterVirtual);

            var result = await _context.Set<DepartmentDetail>().FromSqlRaw("exec Pro_Get_Department_List @CompanyID, @UserID, @FilterVirtual",
                pCompanyID, pUserID, pFilterVirtual).ToListAsync();

            result.Select(async c => {
                c.CreatedByName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
            }).ToList();

            return result;
        }

        public async Task<Department> GetDepartment(int companyId, int departmentId) {
            return await _context.Set<Department>().Where(t => t.CompanyId == companyId && t.DepartmentId == departmentId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateDepartment(Department department, CancellationToken token) {

            var result = _context.Set<Department>().Where(t => t.DepartmentId == department.DepartmentId).FirstOrDefault();

            if (result == null) {
                return default;
            } else {
                result.DepartmentName = department.DepartmentName;
                result.Status = department.Status;
                result.UpdatedOn = department.UpdatedOn;
                result.UpdatedBy = department.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.DepartmentId;
            }
        }

        public bool CheckDuplicate(Department department) {
            return _context.Set<Department>().Where(t => t.DepartmentName.Equals(department.DepartmentName) && t.CompanyId == department.CompanyId).Any();
        }

        public async Task<bool> CheckForExistance(int departmentId) {
            return await _context.Set<Department>().Where(t => t.DepartmentId.Equals(departmentId)).AnyAsync();
        }
        public async Task<int> DepartmentStatus(int CompanyID) {
            try {
                var pCompanyId = new SqlParameter("@CompanyId", CompanyID);

                var result = _context.Set<Result>().FromSqlRaw("exec Pro_Get_Department_Status @CompanyID", pCompanyId).AsEnumerable();
                var id = result.FirstOrDefault().ResultID;
                if (result != null) {
                    return id;
                }
                return 0;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<List<GroupLink>> SegregationLinks(int targetID, string memberShipType, string linkType, int currentUserId, int outUserCompanyId) {
            try {

                var pDepartmentID = new SqlParameter("@DepartmentID", targetID);
                var pMemberShipType = new SqlParameter("@MemberShipType", memberShipType);
                var pLinkType = new SqlParameter("@LinkType", linkType);
                var pUserID = new SqlParameter("@UserID", currentUserId);
                var pCompanyID = new SqlParameter("@CompanyID", outUserCompanyId);

                var result = await _context.Set<GroupLink>().FromSqlRaw("exec Pro_Get_Department_Links @DepartmentID, @LinkType, @MemberShipType, @UserID, @CompanyID",
                    pDepartmentID, pLinkType, pMemberShipType, pUserID, pCompanyID).ToListAsync();
                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<bool> UpdateSegregationLink(int SourceID, int TargetID, GroupType LinkType, int CompanyId) {
            try {
                if (LinkType.ToGrString().ToUpper() == GroupType.GROUP.ToGrString().ToUpper()) {
                    var item = await _context.Set<SegGroupDepartmentLink>().Where(I => I.DepartmentId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();

                    if (item == null) {
                        await CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    } else if (item != null) {
                        _context.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    return true;
                } else if (LinkType.ToGrString().ToUpper() == GroupType.INCIDENT.ToGrString().ToUpper()) {
                    var item = await _context.Set<SegGroupDepartmentLink>().Where(I => I.DepartmentId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                    if (item == null) {
                        await CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    } else if (item != null) {
                        _context.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    return true;
                }

                return false;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task CreateSegregtionLink(int sourceID, int targetID, GroupType LinkType, int companyId) {
            try {
                if (LinkType.ToGrString().ToUpper() == GroupType.GROUP.ToGrString().ToUpper()) {
                    var targtetitem = await _context.Set<Group>().Where(w => w.GroupId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null) {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink() {
                            CompanyId = companyId,
                            DepartmentId = sourceID,
                            GroupId = targetID
                        };
                        await _context.AddAsync(SGL);
                        await _context.SaveChangesAsync();
                    }
                } else if (LinkType.ToGrString().ToUpper() == GroupType.INCIDENT.ToGrString().ToUpper()) {
                    var targtetitem = await _context.Set<Incident>().Where(w => w.IncidentId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null) {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink() {
                            CompanyId = companyId,
                            GroupId = targetID,
                            DepartmentId = sourceID
                        };
                        await _context.AddAsync(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
