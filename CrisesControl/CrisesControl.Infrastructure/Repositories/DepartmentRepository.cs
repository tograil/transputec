﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Common;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Department = CrisesControl.Core.Departments.Department;
using Group = CrisesControl.Core.Groups.Group;

namespace CrisesControl.Infrastructure.Repositories
{
    public class DepartmentRepository: IDepartmentRepository
    {
        private readonly CrisesControlContext _context;

        public DepartmentRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDepartment(Department department, CancellationToken token)
        {
            await _context.AddAsync(department, token);

            await _context.SaveChangesAsync(token);

            return department.DepartmentId;
        }

        public async Task<int> DeleteDepartment(int departmentId, CancellationToken token)
        {
            await _context.AddAsync(departmentId, token);

            await _context.SaveChangesAsync(token);

            return departmentId;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments(int companyId)
        {
            return await _context.Set<Department>().Where(t=>t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Department> GetDepartment(int companyId, int departmentId)
        {
            return await _context.Set<Department>().Where(t => t.CompanyId == companyId && t.DepartmentId == departmentId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateDepartment(Department department, CancellationToken token)
        {

            var result = _context.Set<Department>().Where(t=>t.DepartmentId == department.DepartmentId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.DepartmentName = department.DepartmentName;
                result.Status = department.Status;
                result.UpdatedOn = department.UpdatedOn;
                result.UpdatedBy = department.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.DepartmentId;
            }
        }

        public bool CheckDuplicate(Department department)
        {
            return _context.Set<Department>().Where(t => t.DepartmentName.Equals(department.DepartmentName) && t.CompanyId == department.CompanyId).Any();
        }

        public bool CheckForExistance(int DepartmentId)
        {
            return _context.Set<Department>().Where(t => t.DepartmentId.Equals(DepartmentId)).Any();
        }
        public async Task<int> DepartmentStatus(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", CompanyID);

                var result = await  _context.Set<Result>().FromSqlRaw("exec Pro_Get_Department_Status @CompanyID", pCompanyId).FirstOrDefaultAsync();
                if (result != null)
                {
                    return result.ResultID;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<List<GroupLink>> SegregationLinks(int TargetID, string MemberShipType,string LinkType,int CurrentUserId, int OutUserCompanyId)
        {
            try
            {

                var pDepartmentID = new SqlParameter("@DepartmentID",TargetID);
                var pMemberShipType = new SqlParameter("@MemberShipType", MemberShipType);
                var pLinkType = new SqlParameter("@LinkType", LinkType);
                var pUserID = new SqlParameter("@UserID", CurrentUserId);
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);

                var result = await _context.Set<GroupLink>().FromSqlRaw("exec Pro_Get_Department_Links @DepartmentID, @LinkType, @MemberShipType, @UserID, @CompanyID",
                    pDepartmentID, pLinkType, pMemberShipType, pUserID, pCompanyID).ToListAsync();
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateSegregationLink(int SourceID, int TargetID, string Action, GroupType LinkType,  int CompanyId)
        {
            try
            {
                if (LinkType.ToGrString().ToUpper() == GroupType.GROUP.ToGrString().ToUpper())
                {
                    var item = await _context.Set<SegGroupDepartmentLink>().Where(I=> I.DepartmentId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                    if (Action.ToUpper() == "ADD" && item == null)
                    {
                       await CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    }
                    else if (Action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Remove(item);
                       await _context.SaveChangesAsync();
                    }
                    return true;
                }
                else if (LinkType.ToGrString().ToUpper() == GroupType.INCIDENT.ToGrString().ToUpper())
                {
                    var item = await _context.Set<SegGroupDepartmentLink>().Where(I=> I.DepartmentId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                    if (Action.ToUpper() == "ADD" && item == null)
                    {
                      await  CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    }
                    else if (Action.ToUpper() == "REMOVE" && item != null)
                    {
                        _context.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateSegregtionLink(int sourceID, int targetID, GroupType LinkType, int companyId)
        {
            try
            {
                if (LinkType.ToGrString().ToUpper() == GroupType.GROUP.ToGrString().ToUpper())
                {
                    var targtetitem = await _context.Set<Group>().Where(w => w.GroupId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink()
                        {
                            CompanyId = companyId,
                            DepartmentId = sourceID,
                            GroupId = targetID
                        };
                        await _context.AddAsync(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (LinkType.ToGrString().ToUpper() == GroupType.INCIDENT.ToGrString().ToUpper())
                {
                    var targtetitem = await _context.Set<Incident>().Where(w => w.IncidentId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegGroupDepartmentLink SGL = new SegGroupDepartmentLink()
                        {
                            CompanyId = companyId,
                            GroupId = targetID,
                            DepartmentId = sourceID
                        };
                        await _context.AddAsync(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
