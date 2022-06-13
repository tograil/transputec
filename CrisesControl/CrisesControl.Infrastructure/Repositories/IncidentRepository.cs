﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.SPResponse;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IncidentActivation = CrisesControl.Core.Incidents.IncidentActivation;

namespace CrisesControl.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly CrisesControlContext _context;
    private readonly ICompanyParametersRepository _companyParamentersRepository;
    private readonly IMessageService _service;
    private readonly ILogger<IncidentRepository> _logger;
    public IncidentRepository(CrisesControlContext context, ICompanyParametersRepository companyParamentersRepository, IMessageService service, ILogger<IncidentRepository> logger)
    {
        _context = context;
        _companyParamentersRepository = companyParamentersRepository;
        _service = service;
        _logger = logger;
    }

    public async Task<bool> CheckDuplicate(int companyId, string incidentName, int incidentId)
    {
        if (incidentId == 0)
            return !await _context.Set<Incident>().AnyAsync(x => x.CompanyId == companyId 
                                                                    && x.Name == incidentName && x.Status != 3);

        return await _context.Set<Incident>().AnyAsync(x => x.CompanyId == companyId
                                                 && x.Name == incidentName && x.Status != 3 && x.IncidentId == incidentId);
    }

    public async Task<Incident?> GetIncident(int companyId, int incidentId)
    {
        return await _context.Set<Incident>()
            .FirstOrDefaultAsync(x => x.IncidentId == incidentId && x.CompanyId == companyId);
    }

    public async Task<int> AddIncident(Incident incident)
    {
        await _context.AddAsync(incident);

        await _context.SaveChangesAsync();

        return incident.IncidentId;
    }

    public async Task AddIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken)
    {
        await _context.AddAsync(incidentActivation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken)
    {
        _context.Update(incidentActivation);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IncidentActivation?> GetIncidentActivation(int companyId, int incidentActivationId)
    {
        var result = await _context.Set<IncidentActivation>()
            .Include(x => x.Incident)
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.IncidentActivationId == incidentActivationId);

        return result;
    }

    public async Task AddIncidentKeyContacts(ICollection<IncidentKeyContact> contacts)
    {
        await _context.AddRangeAsync(contacts);

        await _context.SaveChangesAsync();
    }

    public async Task ProcessKeyHolders(int companyId, int incidentId, int currentUserId, int[] keyHolders)
    {
        var incKeyHolders = _context.Set<IncidentKeyholder>()
            .Where(x => x.CompanyID == companyId
                        && x.IncidentID == incidentId
                        && (x.ActiveIncidentID == null ||
                            x.ActiveIncidentID == 0))
            .ToArray();

        var keyList = new List<long>();

        foreach (var keyHolderId in keyHolders)
        {
            if (keyHolderId <= 0) continue;

            var keyHld = incKeyHolders
                .FirstOrDefault(s => s.CompanyID == companyId
                                     && s.IncidentID == incidentId && s.UserID == keyHolderId);
            if (keyHld is not null)
            {
                keyList.Add(keyHld.IncidentKeyholderID);
            }
            else
            {
                var incNewIncKeyHolder = new IncidentKeyholder
                {
                    CompanyID = companyId,
                    IncidentID = incidentId,
                    UserID = keyHolderId
                };

                await _context.AddAsync(incNewIncKeyHolder);
            }
        }

        foreach (var incidentKeyholder in incKeyHolders)
        {
            var isDel = keyList.Any(s => s == incidentKeyholder.IncidentKeyholderID);
            if (!isDel)
            {
                _context.Set<IncidentKeyholder>().Remove(incidentKeyholder);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveIncidentMessageResponse(ICollection<AckOption> ackOptions, int incidentId)
    {
        var prevOptions = await _context.Set<IncidentMessageResponse>()
            .Where(im => im.IncidentId == incidentId)
            .ToListAsync();

        _context.Set<IncidentMessageResponse>().RemoveRange(prevOptions);
        await _context.SaveChangesAsync();

        var incidentMessageToAdd = ackOptions.Select(x =>
            new IncidentMessageResponse
            {
                ResponseId = x.ResponseId,
                ResponseCode = x.ResponseCode,
                IncidentId = incidentId
            })
            .ToList();

        await _context.AddRangeAsync(incidentMessageToAdd);

        await _context.SaveChangesAsync();
    }

    public async Task AddIncidentGroup(int incidentId, int[] groups, int companyId)
    {
        var linksToDel = await _context.Set<SegGroupIncidentLink>()
            .Where(x => x.IncidentId == incidentId)
            .ToListAsync();

        _context.Set<SegGroupIncidentLink>().RemoveRange(linksToDel);

        await _context.SaveChangesAsync();

        var groupsToSave = groups.Select(x => new SegGroupIncidentLink
        {
            CompanyId = companyId,
            GroupId = x,
            IncidentId = incidentId
        }).ToList();

        await _context.AddRangeAsync(groupsToSave);

        await _context.SaveChangesAsync();
    }

    public async Task CreateIncidentSegLinks(int incidentId, int userId, int companyId)
    {
        var pCompanyId = new SqlParameter("@CompanyId", companyId);
        var pUserId = new SqlParameter("@UserId", userId);
        var pIncidentId = new SqlParameter("@IncidentId", incidentId);

        await _context.Database.ExecuteSqlRawAsync("exec Pro_Incident_CreateLink @CompanyId, @UserId, @IncidentId", pCompanyId, pUserId, pIncidentId);
    }

    public NewIncident CloneIncident(int incidentId, bool keepKeyContact, bool keepIncidentMessage, bool keepTasks,
        bool keepIncidentAsset, bool keepTaskAssets, bool keepTaskCheckList, bool keepIncidentParticipants, int status,
        int currentUserId, int companyId, string timeZoneId)
    {
        var pIncidentId = new SqlParameter("@IncidentID", incidentId);
        var pKeepKeyContact = new SqlParameter("@KeepKeyContact", keepKeyContact);
        var pKeepIncidentMessage = new SqlParameter("@KeepIncidentMessage", keepIncidentMessage);
        var pKeepTasks = new SqlParameter("@KeepTasks", keepTasks);
        var pKeepIncidentAsset = new SqlParameter("@KeepIncidentAsset",keepIncidentAsset);
        var pKeepTaskAssets = new SqlParameter("@KeepTaskAssets", keepTaskAssets);
        var pKeepTaskCheckList = new SqlParameter("@KeepTaskCheckList", keepTaskCheckList);
        var pKeepIncidentParticipants = new SqlParameter("@KeepIncidentParticipants", keepIncidentParticipants);
        var pStatus = new SqlParameter("@Status", status);
        var pCurrentUserId = new SqlParameter("@CurrentUserID", currentUserId);
        var pCompanyId = new SqlParameter("@CompanyID", companyId);
        var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset(timeZoneId));

        var result = _context.Set<NewIncident>().FromSqlRaw("exec Pro_Copy_Incident {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
            pIncidentId, pKeepKeyContact, pKeepIncidentMessage, pKeepTasks, pKeepIncidentAsset, pKeepTaskAssets, pKeepTaskCheckList, pKeepIncidentParticipants, pStatus, pCurrentUserId, pCompanyId, pCustomerTime).ToList().First();

        return result;
    }

    public async Task<ICollection<DataIncidentType>> CopyIncidentTypes(int userId, int companyId)
    {
        var libIncidentTypeData = await _context.Set<LibIncidentType>().ToListAsync();

        var dataIncidentTypes = new List<DataIncidentType>();

        foreach (var libIncidentType in libIncidentTypeData)
        {
            var incType = await _context.Set<IncidentType>()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && libIncidentType.Name == x.Name);

            if (incType is not null)
            {
                dataIncidentTypes.Add(new DataIncidentType
                {
                    IncidentTypeId = incType.IncidentTypeId,
                    Name = incType.Name!,
                    LibIncidentType = libIncidentType.LibIncidentTypeId
                });
            }
            else
            {
                var newIncidentType = new IncidentType()
                {
                    CompanyId = companyId,
                    Name = libIncidentType.Name,
                    Status = 1
                };

                await _context.AddAsync(newIncidentType);
                await _context.SaveChangesAsync();

                dataIncidentTypes.Add(new DataIncidentType
                {
                    IncidentTypeId = newIncidentType.IncidentTypeId,
                    Name = newIncidentType.Name,
                    LibIncidentType = libIncidentType.LibIncidentTypeId
                });
            }
        }

        return dataIncidentTypes;
    }

    public async Task CopyIncidentToCompany(int companyId, int userId, string timeZoneId = "GMT Standard Time")
    {
        var sourceIncidentTypes = await CopyIncidentTypes(userId, companyId);

        var incidentsFromLib =
            await _context.Set<LibIncident>().Where(x => x.IsDefault == 1 && x.Status != 3).ToListAsync();

        foreach (var incident in incidentsFromLib)
        {
            var isIncidentExists = await _context.Set<Incident>()
                .AnyAsync(x => x.CompanyId == companyId && x.Name == incident.Name);

            if (!isIncidentExists)
            {
                var incidentType =
                    sourceIncidentTypes.FirstOrDefault(x => x.LibIncidentType == incident.LibIncidentTypeId);

                if (incidentType != null)
                {
                    var keyHolderList = _context.Set<User>().Where(x => x.CompanyId == companyId && x.Status != 3)
                        .Select(x => new AddIncidentKeyHldLst(x.UserId));

                    var ackOptions = new List<AckOption>();

                    var incidentNew = new Incident
                    {
                        CompanyId = companyId,
                        IncidentIcon = incident.LibIncodentIcon,
                        Name = incident.Name,
                        Description = incident.Description,
                        PlanAssetId = 0,
                        IncidentTypeId = incidentType.IncidentTypeId,
                        Severity = incident.Severity,
                        Status = incident.Status,
                        NumberOfKeyHolders = 1,
                        AudioAssetId = 0,
                        TrackUser = false,
                        SilentMessage = false,
                        CreatedBy = userId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        UpdatedBy = userId,
                        IsSos = false,
                        CascadePlanId = 0,
                        UpdatedOn = DateTimeOffset.UtcNow
                    };

                    var incidentId = await AddIncident(incidentNew);

                    var contacts = keyHolderList
                        .Select(x => new IncidentKeyContact
                        {
                            CompanyId = companyId,
                            IncidentId = incidentId,
                            UserId = x.UserId ?? 0,
                            CreatedBy = userId,
                            CreatedOn = DateTimeOffset.UtcNow,
                            UpdatedBy = userId,
                            UpdatedOn = DateTimeOffset.UtcNow
                        }).ToArray();

                    await AddIncidentKeyContacts(contacts);

                    await ProcessKeyHolders(companyId, incidentId, userId, Array.Empty<int>());

                    await SaveIncidentMessageResponse(ackOptions, incidentId);

                    await AddIncidentGroup(incidentId, Array.Empty<int>(), companyId);

                    await CreateIncidentSegLinks(incidentId, userId, companyId);
                }
            }
        }
    }

    public async Task<string> GetStatusName(int status)
    {
        return (await _context.Set<SysParameter>()
            .FirstAsync(x => x.Value == status.ToString()
                             && x.Category == "IncidentStatus")).Name;
    }

    public List<IncidentList> GetCompanyIncident(int CompanyId, int UserID)
    {
        var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
        var pUserID = new SqlParameter("@UserID", UserID);
        return _context.Set<IncidentList>().FromSqlRaw("Pro_Get_Company_Incident @CompanyID,@UserID", pCompanyId, pUserID).ToList();
    }

    public List<IncidentTypeReturn> CompanyIncidentType(int CompanyId)
    {
        return (from IncidentTypeval in _context.Set<IncidentType>().AsEnumerable()
                where IncidentTypeval.CompanyId == CompanyId && IncidentTypeval.Status == 1
                orderby IncidentTypeval.Name
                select new IncidentTypeReturn
                {
                    IncidentTypeId = IncidentTypeval.IncidentTypeId,
                    Companyid = IncidentTypeval.CompanyId,
                    Name = IncidentTypeval.Name,
                    Status = IncidentTypeval.Status
                }).ToList();
    }

    public List<AffectedLocation> GetAffectedLocation(int CompanyId, string? LocationType)
    {
        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
        var pLocationType = new SqlParameter("@LocationType", LocationType);
        return _context.Set<AffectedLocation>().FromSqlRaw("Pro_ActiveIncident_GetAffectedLocation @CompanyID, @LocationType",
            pCompanyID, pLocationType).ToList();
    }

    public List<AffectedLocation> GetIncidentLocation(int CompanyId, int IncidentActivationId)
    {
        var locs = (from IL in _context.Set<IncidentLocation>().AsEnumerable()
                    join ILL in _context.Set<IncidentLocationLibrary>().AsEnumerable() on IL.LibLocationId equals ILL.LocationId
                    where IL.CompanyId == CompanyId && IL.IncidentActivationId == IncidentActivationId
                    select new AffectedLocation
                    {
                        Address = ILL.Address,
                        Lat = ILL.Lat,
                        Lng = ILL.Lng,
                        LocationID = ILL.LocationId,
                        LocationName = ILL.LocationName,
                        LocationType = ILL.LocationType
                    }).ToList();
        var incloc = (from L in _context.Set<CrisesControl.Core.Locations.Location>().AsEnumerable()
                        join IA in _context.Set<IncidentActivation>().AsEnumerable() on L.LocationId equals IA.ImpactedLocationId
                        where IA.IncidentActivationId == IncidentActivationId
                        select new AffectedLocation
                        {
                            Address = L.PostCode,
                            Lat = L.Lat,
                            Lng = L.Long,
                            LocationID = L.LocationId,
                            LocationName = L.LocationName,
                            LocationType = "IMPACTED"
                        }).ToList();

        return locs.Union(incloc).Distinct().ToList();
    }

    public List<CommsMethods> GetIncidentComms(int ItemID, string Type)
    {
        List<CommsMethods> result = new();
        if (Type == "TASK" && ItemID > 0)
        {
            ItemID = (from TI in _context.Set<TaskActiveIncident>().AsEnumerable() where TI.ActiveIncidentTaskId == ItemID select TI.ActiveIncidentId).FirstOrDefault();
        }

        //Use: [dbo].[Pro_ActiveIncident_GetMessageMethods] @ItemID
        result = (from MM in _context.Set<CrisesControl.Core.Messages.MessageMethod>().AsEnumerable()
                        join MT in _context.Set<CommsMethod>().AsEnumerable() on MM.MethodId equals MT.CommsMethodId
                        where MM.ActiveIncidentId == ItemID
                        select new CommsMethods { MethodId = MM.MethodId, MethodName = MT.MethodName }).ToList();
        return result;
    }

    public IncidentDetails GetIncidentById(int CompanyId, int UserID, int IncidentId, string UserStatus = "ACTIVE")
    {
        IncidentDetails result = new();

        try
        {
            //Use:  EXEC [dbo].[Pro_Incident_GetIncidentByRef] @CompanyID,@UserID,@IncidentID
            //      EXEC [dbo].[Pro_Incident_GetIncidentByRef_ActionList] @CompanyID,@IncidentID (for ActList)
            //      EXEC [dbo].[Pro_Incident_GetIncidentByRef_KeyConList] @CompanyID,@IncidentID,@UserStatus (for incikeycon)
            //      EXEC [dbo].[Pro_Incident_GetIncidentByRef_AssetList] @CompanyID,@IncidentID (for IncidentAssets)
            //      EXEC [dbo].[Pro_Incident_GetIncidentByRef_Messagemethods] @CompanyID,@IncidentID (for MessageMethods subquery)
            //      EXEC [dbo].[Pro_Incident_GetIncidentByRef_AckOptions] @IncidentID (for AckOptions subquery)

            //_context.Set<TaskActiveIncident>().AsEnumerable()

            var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
            //result = _context.Set<IncidentDetails>().FromSqlRaw("Pro_Incident_GetIncidentByRef @CompanyID,@UserID,@IncidentID", pCompanyID, pUserID, pIncidentID).FirstOrDefault();
            result = _context.Set<IncidentDetails>().FromSqlRaw("Pro_Incident_GetIncidentByRef @CompanyID,@UserID,@IncidentID", pCompanyID, pUserID, pIncidentID).ToList().FirstOrDefault();



            //var pIncidentID1 = new SqlParameter("@IncidentID", IncidentId);
            //result.Groups = db.Database.SqlQuery<IncidentGroup>("Pro_Incident_GetIncidentByRef_Groups @IncidentID", pIncidentID1).ToList();

            //var pLocInc = new SqlParameter("@IncidentID", IncidentId);
            //result.SharingLocations = db.Database.SqlQuery<IncidentSharingLocation>("Pro_Incident_GetIncidentByRef_Location @IncidentID", pLocInc).ToList();

            //var pDepInc = new SqlParameter("@IncidentID", IncidentId);
            //result.SharingDepartments = db.Database.SqlQuery<IncidentSharingDepartment>("Pro_Incident_GetIncidentByRef_Department @IncidentID", pDepInc).ToList();



            var pCompanyID2 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID2 = new SqlParameter("@IncidentID", IncidentId);
            result.MessageMethods = _context.Set<CommsMethods>().FromSqlRaw("Pro_Incident_GetIncidentByRef_Messagemethods @CompanyID,@IncidentID", pCompanyID2, pIncidentID2).ToList();

            var pIncidentID3 = new SqlParameter("@IncidentID", IncidentId);
            result.AckOptions = _context.Set<AckOption>().FromSqlRaw("Pro_Incident_GetIncidentByRef_AckOptions @IncidentID", pIncidentID3).ToList();

            var pCompanyID4 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID4 = new SqlParameter("@IncidentID", IncidentId);
            var pUserStatus = new SqlParameter("@UserStatus", UserStatus ?? "ACTIVE");
            result.IncKeyCon = _context.Set<IncKeyCons>().FromSqlRaw("Pro_Incident_GetIncidentByRef_KeyConList @CompanyID,@IncidentID,@UserStatus", pCompanyID4, pIncidentID4, pUserStatus).ToList();

            var pCompanyID5 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID5 = new SqlParameter("@IncidentID", IncidentId);
            result.IncidentAssets = _context.Set<IncidentAssetResponse>().FromSqlRaw("Pro_Incident_GetIncidentByRef_AssetList @CompanyID,@IncidentID", pCompanyID5, pIncidentID5).ToList();

            var pCompanyID6 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID6 = new SqlParameter("@IncidentID", IncidentId);
            result.ActionLst = _context.Set<ActionLsts>().FromSqlRaw(
                "Pro_Incident_GetIncidentByRef_ActionList @CompanyID,@IncidentID", pCompanyID6, pIncidentID6).AsEnumerable();

            var pCompanyID7 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID7 = new SqlParameter("@IncidentID", IncidentId);
            var pIncidentActionID7 = new SqlParameter("@IncidentActionID", -1);
            result.Participants = _context.Set<IncidentParticipants>().FromSqlRaw("Pro_Incident_GetIncidentByRef_Participant @CompanyID, @IncidentID, @IncidentActionID",
                pCompanyID7, pIncidentID7, pIncidentActionID7).ToList();

            //_context.Set<IncKeyCons>().FromSqlRaw("Pro_Incident_GetIncidentByRef_KeyholderList
            if (result.HasNominatedKeyholders)
            {
                var pCompanyID8 = new SqlParameter("@CompanyID", CompanyId);
                var pIncidentID8 = new SqlParameter("@IncidentID", IncidentId);
                var pUserStatus1 = new SqlParameter("@UserStatus", UserStatus ?? "ACTIVE");
                result.IncKeyholders = _context.Set<IncKeyCons>().FromSqlRaw("Pro_Incident_GetIncidentByRef_KeyholderList @CompanyID,@IncidentID,@UserStatus", pCompanyID8, pIncidentID8, pUserStatus1).ToList();
            }

            return result;
        }
        catch (Exception ex)
        {
        }
        return new IncidentDetails();
    }
    public async Task<int> ActivateSampleIncident(int UserID, int CompanyID, string TimeZoneID)
    {
        try
        {
            var check_incident = await _context.Set<Incident>().Where(I=> I.CompanyId == CompanyID).FirstOrDefaultAsync();
            if (check_incident == null)
            {
               IEnumerable<DataIncidentType> source_incident_type = await CopyIncidentTypes(UserID, CompanyID);

                int sample_incident_id = 986;
                bool checkid = int.TryParse(await LookupWithKey("SAMPLE_INCIDENT_ID"), out sample_incident_id);

                var sample_incident = await _context.Set<LibIncident>().Where(LI=> LI.LibIncidentId == sample_incident_id).FirstOrDefaultAsync();

                var reco = source_incident_type.Where(myrow=>myrow.IncidentTypeId == sample_incident.LibIncidentId).FirstOrDefault();
                int toIncidentTypeId = reco.IncidentTypeId;

               

                //Create new sample incident
                int incidentId = await AddCompanyIncidents(CompanyID, sample_incident.LibIncodentIcon, sample_incident.Name, sample_incident.Description, 0,
                    toIncidentTypeId, sample_incident.Severity, 1, UserID, TimeZoneID, null, 0, sample_incident.Status);


                //Attach the key contacts
                var KeyHolderList = await _context.Set<User>().Where(U=> U.CompanyId == CompanyID && U.Status != 3).Select(U => new AddIncidentKeyHldLst( U.UserId )).Take(2).ToArrayAsync();
                await AttachKeyContactsToIncident(incidentId, UserID, CompanyID, KeyHolderList, TimeZoneID);
                return incidentId;
            }
            return check_incident.IncidentId;

        }
        catch (Exception ex)
        {
            throw ex;
            return 0;
        }
    }

    public async Task<int> AddCompanyIncidents(
        int CompanyId, string IncidentIcon, string Name, string Description, int PlanAssetID,
            int IncidentTypeId, int Severity, int NumberOfKeyHolders, int CurrentUserId, string TimeZoneId,
            AddIncidentKeyHldLst[] AddIncidentKeyHldLst, int AudioAssetId, int Status = 1, bool TrackUser = false,
            bool SilentMessage = false, List<AckOption> AckOptions = null, bool IsSOS = false, int[] MessageMethod = null, int CascadePlanID = 0,
            int[] Groups = null, int[] Keyholders = null)
    {
        string allow_nominated_kh = await _companyParamentersRepository.GetCompanyParameter("ALLOW_KEYHOLDER_NOMINATION", CompanyId);

        Incident tblIncident = new Incident()
        {
            CompanyId = CompanyId,
            IncidentIcon = IncidentIcon,
            Name = Name,
            Description = Description,
            PlanAssetId = PlanAssetID,
            IncidentTypeId = IncidentTypeId,
            Severity = Severity,
            Status = Status,
            NumberOfKeyHolders = NumberOfKeyHolders,
            AudioAssetId = AudioAssetId,
            TrackUser = TrackUser,
            SilentMessage = SilentMessage,
            CreatedBy = CurrentUserId,
            CreatedOn = DateTime.Now,
            UpdatedBy = CurrentUserId,
            IsSos = IsSOS,
            CascadePlanId = CascadePlanID,
            UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId, System.DateTime.Now),
            HasTask = (Keyholders.Length > 0 && NumberOfKeyHolders > 1 && allow_nominated_kh == "true")
        };
       await _context.AddAsync(tblIncident);
       await _context.SaveChangesAsync();


        if (tblIncident.IncidentId > 0)
        {
            if (AddIncidentKeyHldLst != null)
            {
                List<AddIncidentKeyHldLst> LstKeyHld = new List<AddIncidentKeyHldLst>(AddIncidentKeyHldLst);
                foreach (var IKeyHld in LstKeyHld)
                {
                    if (IKeyHld.UserId != null)
                    {
                        IncidentKeyContact tblIncKeyContact = new IncidentKeyContact()
                        {
                            CompanyId = CompanyId,
                            IncidentId = tblIncident.IncidentId,
                            UserId = IKeyHld.UserId.Value,
                            CreatedBy = CurrentUserId,
                            CreatedOn = System.DateTime.Now,
                            UpdatedBy = CurrentUserId,
                            UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                        };
                        await _context.AddAsync(tblIncKeyContact);
                        await _context.SaveChangesAsync();
                    }
                }
            }

           await ProcessKeyholders(CompanyId, tblIncident.IncidentId, CurrentUserId, Keyholders);

            if (AckOptions != null)
            {
               await SaveIncidentMessageResponse(AckOptions, tblIncident.IncidentId);
            }

            if (Groups != null)
            {
               await AddIncidentGroup(tblIncident.IncidentId, Groups, CompanyId);
            }

            if (MessageMethod != null && CascadePlanID <= 0)
            {
                if (MessageMethod.Length > 0)
                {
                    
                    foreach (int Method in MessageMethod)
                    {
                       await _service.CreateMessageMethod(0, Method, 0, tblIncident.IncidentId);
                    }
                }
            }
            //Create incident segregation links
           await CreateIncidentSegLinks(tblIncident.IncidentId, CurrentUserId, CompanyId);

            return tblIncident.IncidentId;
        }
        return 0;
    }

    public async Task AttachKeyContactsToIncident(int IncidentID, int UserID, int CompanyID, AddIncidentKeyHldLst[] KCList, string TimeZoneID)
    {
        try
        {
            if (KCList != null)
            {
                List<AddIncidentKeyHldLst> LstKeyHld = new List<AddIncidentKeyHldLst>(KCList);
                foreach (var IKeyHld in LstKeyHld)
                {
                    if (IKeyHld.UserId != null)
                    {
                        IncidentKeyContact tblIncKeyContact = new IncidentKeyContact()
                        {
                            CompanyId = CompanyID,
                            IncidentId = IncidentID,
                            UserId = IKeyHld.UserId.Value,
                            CreatedBy = UserID,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = UserID,
                            UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneID, DateTime.Now)
                        };
                        await _context.AddAsync(tblIncKeyContact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task ProcessKeyholders(int CompanyId, int IncidentId, int CurrentUserId, int[] Keyholders)
    {
        var incKeyHlds = await _context.Set<IncidentKeyholder>()
                        .Where(incKey=>incKey.CompanyID == CompanyId && incKey.IncidentID == IncidentId 
                        && (incKey.ActiveIncidentID == null ||
                          incKey.ActiveIncidentID == 0)
                          ).ToListAsync();

        List<long> KEYLIST = new List<long>();

        foreach (int keyhld in Keyholders)
        {
            if (keyhld > 0)
            {
                var ISExist = incKeyHlds.FirstOrDefault(s => s.CompanyID == CompanyId && s.IncidentID == IncidentId && s.UserID == keyhld);
                if (ISExist == null)
                {
                    IncidentKeyholder tblIncKeyholder = new IncidentKeyholder()
                    {
                        CompanyID = CompanyId,
                        IncidentID = IncidentId,
                        UserID = keyhld
                    };
                  await  _context.AddAsync(tblIncKeyholder);
                }
                else
                {
                    KEYLIST.Add(ISExist.IncidentKeyholderID);
                }
            }
        }
        foreach (var Ditem in incKeyHlds)
        {
            bool ISDEL = KEYLIST.Any(s => s == Ditem.IncidentKeyholderID);
            if (!ISDEL)
            {
                _context.Remove(Ditem);
            }
        }
      await  _context.SaveChangesAsync();
    }
    private async Task<string> LookupWithKey(string Key, string Default = "")
    {
        try
        {
            Dictionary<string, string> Globals = CCConstants.GlobalVars;
            if (Globals.ContainsKey(Key))
            {
                return Globals[Key];
            }


            var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
            if (LKP != null)
            {
                Default = LKP.Value;
            }
            return Default;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            return Default;
        }
    }
    public async Task<int> CreateSOSIncident(int UserID, int CompanyID, string TimeZoneID)
    {
        try
        {
            IEnumerable<DataIncidentType> source_incident_type = await CopyIncidentTypes(UserID, CompanyID);

            var sos_incident = await _context.Set<LibIncident>().Where(I=> I.IsSos == true).FirstOrDefaultAsync();

            var reco =  source_incident_type.AsEnumerable().Where(myrow=> myrow.IncidentTypeId == sos_incident.LibIncidentTypeId
                       ).FirstOrDefault();

            int toIncidentTypeId = reco.IncidentTypeId;

           

            int[] Methods =  _context.Set<CompanyComm>().Where(CM=> CM.CompanyId == CompanyID).Select (CM=>CM.MethodId).ToList().ToArray();

            //Create new sample incident
            int incidentId = await AddCompanyIncidents(CompanyID, sos_incident.LibIncodentIcon, sos_incident.Name,
                sos_incident.Description, 0, toIncidentTypeId, sos_incident.Severity, 1, UserID, TimeZoneID,
                null, 0, sos_incident.Status, true, false, null, (bool)sos_incident.IsSos, Methods);

            //Attach the key contacts
            var KeyHolderList = await _context.Set<User>().Where(U => U.CompanyId == CompanyID && U.Status != 3).Select(U => new AddIncidentKeyHldLst(U.UserId)).Take(2).ToArrayAsync();
            foreach (AddIncidentKeyHldLst kc in KeyHolderList)
            {
                IncidentKeyContact tblIncKeyContact = new IncidentKeyContact()
                {
                    CompanyId = CompanyID,
                    IncidentId = incidentId,
                    UserId = kc.UserId.Value,
                    CreatedBy = UserID,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = UserID,
                    UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneID, DateTime.Now)
                };
               await _context.AddAsync(tblIncKeyContact);
               await _context.SaveChangesAsync();
            }

            return incidentId;

        }
        catch (Exception ex)
        {
            throw ex;
            return 0;
        }
    }

}