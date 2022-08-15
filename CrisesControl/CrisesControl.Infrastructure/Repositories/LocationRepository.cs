using CrisesControl.Core.Groups;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Group = CrisesControl.Core.Groups.Group;

namespace CrisesControl.Infrastructure.Repositories
{
    public class LocationRepository: ILocationRepository
    {
        private readonly CrisesControlContext _context;

        public LocationRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateLocation(Location location, CancellationToken token)
        {
            await _context.AddAsync(location, token);
            await _context.SaveChangesAsync(token);
            return location.LocationId;
        }

        public async Task<int> DeleteLocation(int locationId, CancellationToken token)
        {
            await _context.AddAsync(locationId);

            await _context.SaveChangesAsync(token);

            return locationId;
        }

        public async Task<IEnumerable<Location>> GetAllLocations(int companyId)
        {
            return await _context.Set<Location>().AsNoTracking().Where(t=>t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Location> GetLocationById(int locationId)
        {
            return await _context.Set<Location>().AsNoTracking().Where(t => t.LocationId == locationId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateLocation(Location location, CancellationToken token)
        {
            var result = _context.Set<Location>().Where(t => t.LocationId == location.LocationId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.LocationName = location.LocationName;
                result.Status = location.Status;
                result.UpdatedOn = location.UpdatedOn;
                result.UpdatedBy = location.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.LocationId;
            }
        }

        public bool CheckDuplicate(Location location)
        {
           return _context.Set<Location>().Where(t => t.LocationName.Equals(location.LocationName)).Any();
        }

        public bool CheckForExisting(int locationId)
        {
            return _context.Set<Location>().Where(t => t.LocationId.Equals(locationId)).Any();
        }
        public async Task<bool> DeleteLocation(int LocationId, int CompanyId, int UserId, string TimeZoneId = "GMT Standard Time")
        {
          
            try
            {
                var Locationdata = await  _context.Set<Location>()
                                    .Where(Locationval=> Locationval.CompanyId == CompanyId && Locationval.LocationId == LocationId
                                    ).FirstOrDefaultAsync();

                if (Locationdata != null)
                {
                    Locationdata.Status = 3;
                    Locationdata.LocationName = "DEL_" + Locationdata.LocationName;
                    Locationdata.UpdatedBy = UserId;
                    Locationdata.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId, DateTime.Now);
                    _context.Update(Locationdata);
                   await _context.SaveChangesAsync();

                    var pLocationId = new SqlParameter("@LocationId", LocationId);
                    var DelOBjs = _context.Set<ObjectRelation>().FromSqlRaw("exec Pro_Get_Objects @LocationId", pLocationId).ToListAsync();
                    _context.RemoveRange(DelOBjs);
                    await _context.SaveChangesAsync();
                    return true;
                }
               
                    return false;
               
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<GroupLink>> SegregationLinks(int TargetID,string MemberShipType,string LinkType,int CurrentUserId, int OutUserCompanyId)
        {
            try
            {

                var pLocationID = new SqlParameter("@LocationID",TargetID);
                var pMemberShipType = new SqlParameter("@MemberShipType", MemberShipType);
                var pLinkType = new SqlParameter("@LinkType", LinkType);
                var pUserID = new SqlParameter("@UserID", CurrentUserId);
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);

                var result = await _context.Set<GroupLink>().FromSqlRaw("exec Pro_Get_Location_Links @LocationID, @LinkType, @MemberShipType, @UserID, @CompanyID",
                    pLocationID, pLinkType, pMemberShipType, pUserID, pCompanyID).ToListAsync();
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> UpdateSegregationLink(int SourceID, int TargetID, string Action, GroupType LinkType, int CompanyId)
        {
            try
            {
                if (LinkType.ToGrString() == GroupType.GROUP.ToGrString())
                {
                    var item = await _context.Set<SegGroupLocationLink>().Where(I=> I.LocationId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                    if (Action.ToUpper() == "ADD" || item == null)
                    {
                       await  CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    }
                    else if (Action.ToUpper() == "REMOVE" || item != null)
                    {
                        _context.Remove(item);
                       await  _context.SaveChangesAsync();
                    }
                }
                else if (LinkType.ToGrString() == GroupType.INCIDENT.ToGrString())
                {
                    var item = await _context.Set<SegGroupLocationLink>().Where(I => I.LocationId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                    if (Action.ToUpper() == "ADD" || item == null)
                    {
                        await CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                    }
                    else if (Action.ToUpper() == "REMOVE" || item != null)
                    {
                        _context.Remove(item);
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
        private async Task CreateSegregtionLink(int sourceID, int targetID, GroupType LinkType, int companyId)
        {
            try
            {
                if (LinkType.ToGrString() == GroupType.GROUP.ToGrString())
                {
                    var targtetitem = await _context.Set<Group>().Where(w => w.GroupId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegIncidentLocationLink SGL = new SegIncidentLocationLink()
                        {
                            CompanyId = companyId,
                            LocationId = sourceID,
                            IncidentId = targetID
                        };
                        await _context.AddAsync(SGL);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (LinkType.ToGrString() == GroupType.INCIDENT.ToGrString())
                {
                    var targtetitem = await _context.Set<Incident>().Where(w => w.IncidentId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                    if (targtetitem != null)
                    {
                        SegIncidentLocationLink SGL = new SegIncidentLocationLink()
                        {
                            CompanyId = companyId,
                            IncidentId = targetID,
                            LocationId = sourceID
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
