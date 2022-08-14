using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.SPResponse;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CrisesControl.Core.Reports;
using IncidentActivation = CrisesControl.Core.Incidents.IncidentActivation;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Exceptions.NotFound;
using System.Data.SqlTypes;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages;
using MessageMethod = CrisesControl.Core.Messages.MessageMethod;
using CrisesControl.Core.Compatibility;

namespace CrisesControl.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly CrisesControlContext _context;
    private readonly ICompanyParametersRepository _companyParamentersRepository;
    private readonly IMessageService _service;
    private readonly ILogger<IncidentRepository> _logger;
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public bool IsSOS = false;
    public string Latitude = "0";
    public string Longtude = "0";
    public bool IsFundAvailable = true;
    private readonly IActiveIncidentTaskService _activeIncidentTaskService;
    public IncidentRepository(CrisesControlContext context, IActiveIncidentRepository activeIncidentRepository, ICompanyParametersRepository companyParamentersRepository, IMessageService service, ILogger<IncidentRepository> logger, IHttpContextAccessor httpContextAccessor, IActiveIncidentTaskService activeIncidentTaskService)
    {
        _context = context;
        _companyParamentersRepository = companyParamentersRepository;
        _service = service;
        _logger = logger;
        _activeIncidentRepository = activeIncidentRepository;
        _httpContextAccessor = httpContextAccessor;
        _activeIncidentTaskService = activeIncidentTaskService;
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
    public async Task<List<IncidentTask>> GetNotes(int ObjectID, string NoteType, bool GetAttachments, string AttachmentType, int CompanyId)
    {
        try
        {
            var pObjectID = new SqlParameter("@ObjectID", ObjectID);
            var pNoteType = new SqlParameter("@NoteType", NoteType);
            var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
            var result = await _context.Set<IncidentTask>().FromSqlRaw("exec Pro_Get_Notes @ObjectID, @NoteType, @CompanyID", pObjectID, pNoteType, pCompanyId).ToListAsync();
                result.Select(async c => {
                if (GetAttachments)
                {
                    c.Attachments =await _get_attachments(c.IncidentTaskNotesId, AttachmentType);
                }
                return c;

            }).ToList();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<Attachment>> _get_attachments(int ObjectID, string AttachmentType)
    {
        try
        {
            var pObjectID = new SqlParameter("@ObjectID", ObjectID);
            var pAttachmentType = new SqlParameter("@AttachmentType", AttachmentType);

            var result =await  _context.Set<Attachment>().FromSqlRaw("Attachments_Select @ObjectID, @AttachmentType", pObjectID, pAttachmentType).ToListAsync();
            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<Attachment>> GetAttachments(int ObjectID, string AttachmentType)
    {
        return await _get_attachments(ObjectID, AttachmentType);
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
    public async Task CheckSOSIncident(int CompanyID, int UserID, string TimeZoneID)
    {
        try
        {
            var incident = await _context.Set<Incident>()
                            .Where(I=> I.IsSos == true && I.CompanyId == CompanyID
                            ).AnyAsync();
            if (!incident)
            {
               await CreateSOSIncident(UserID, CompanyID, TimeZoneID);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<IncidentMessagesRtn>> GetIndidentTimeline(int IncidentActivationID, int CompanyID, int UserID)
    {

        try
        {

            var pIncidentActivationID = new SqlParameter("@IncidentActivationID", IncidentActivationID);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);

            var incimsgs = await _context.Set<IncidentMessagesRtn>().FromSqlRaw("exec Pro_Get_Incident_Timeline @IncidentActivationID, @CompanyID, @UserID",
              pIncidentActivationID, pCompanyID, pUserID)
              .ToListAsync();
              incimsgs.Select(c => {
                  c.SentBy = new UserFullName { Firstname = c.SentByFirst, Lastname = c.SentByLast };
                  //c.Notes =  _context.Set<IncidentTaskNote>()
                   //          .Where(N=>(N.ObjectId == IncidentActivationID && N.NoteType == "TASK")
                    //         || N.ObjectId == IncidentActivationID && N.NoteType == "INCIDENT" && c.MessageType == "Ping").FirstOrDefault();
                  //TODO invalid objects
                  return c;
              }).ToList();
           
                return incimsgs;
         
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> AddIncidentNote(int ActiveIncidentID, string Note, List<Attachment> Attachments, int UserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            var inci =await  _context.Set<IncidentActivation>().Where(I=> I.IncidentActivationId == ActiveIncidentID).FirstOrDefaultAsync();
            if (inci != null)
            {
                int NoteId = await IncidentNote(ActiveIncidentID, "INCIDENT", Note, CompanyID, UserID);
                if (NoteId > 0)
                {
                    inci.HasNotes = true;
                    inci.UpdatedBy = UserID;
                    inci.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    _context.Update(inci);
                   await _context.SaveChangesAsync();

                    if (Attachments.Count > 0)
                    {
                        foreach (var attach in Attachments)
                        {
                          await  InsertAttachment(NoteId, attach.Title, attach.OrigFileName, attach.FileName, attach.MimeType, attach.AttachmentType, attach.FileSize);
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
        
    }
    public async Task<int> IncidentNote(int ObjectID, string NoteType, string Notes, int CompanyID, int UserID)
    {
        try
        {
            IncidentTaskNote Note = new IncidentTaskNote()
            {
                UserId = UserID,
                ObjectId = ObjectID,
                CompanyId = CompanyID,
                IncidentTaskNotesId = ObjectID,
                NoteType = NoteType,
                Notes = Notes,
                CreatedDate = DateTime.Now,
            };
           await _context.AddAsync(Note);
           await _context.SaveChangesAsync();
            return Note.IncidentTaskNotesId;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> InsertAttachment(int ObjectID, string Title, string OrigFileName, string FileName, string MimeType, string AttachmentType, decimal? FileSize)
    {
        try
        {
           

                var pObjectID = new SqlParameter("@ObjectID", ObjectID);
                var pTitle = new SqlParameter("@Title", Title);
                var pOrigFileName = new SqlParameter("@OrigFileName", OrigFileName);
                var pFileName = new SqlParameter("@FileName", FileName);
                var pMimeType = new SqlParameter("@MimeType", MimeType);
                var pAttachmentType = new SqlParameter("@AttachmentType", AttachmentType);
                var pFileSize = new SqlParameter("@FileSize", FileSize);

               await _context.Database.ExecuteSqlRawAsync("Attachment_Insert @ObjectID, @Title, @OrigFileName, @FileName, @MimeType, @AttachmentType, @FileSize",
                    pObjectID, pTitle, pOrigFileName, pFileName, pMimeType, pAttachmentType, pFileSize);
                return true;
            
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }
    public async Task<CallToAction> GetCallToAction(int ActiveIncidentID, int UserID, int CompanyID, string TimeZoneId)
    {
        try
        {
      
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUserID = new SqlParameter("@UserID", UserID);
                var pTimeZone = new SqlParameter("@TimeZoneId", TimeZoneId);

                var result = await _context.Set<CallToAction>().FromSqlRaw("exec Pro_Get_Call_To_Action_Items @ActiveIncidentID, @CompanyID, @UserID, @TimeZoneId",
                    pActiveIncidentID, pCompanyID, pUserID, pTimeZone).AsQueryable().ToListAsync();

                return result.FirstOrDefault();
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> UpdateSOS(int SOSAlertID, int UserID, string SOSClosureNotes, bool CloseSOS, bool CloseAllSOS,
            bool MultiNotes, int[] CaseNoteIDs, bool CloseSOSIncident, int ActiveIncidentID, int CurrentUserId, int CompanyId, string TimeZoneId)
    {
        try
        {

            if (MultiNotes && CaseNoteIDs != null)
            {
                foreach (int AlertID in CaseNoteIDs)
                {
                    await IncidentNote(AlertID, "SOSALERT", SOSClosureNotes, CompanyId, CurrentUserId);
                }
            }
            else
            {
                await IncidentNote(SOSAlertID, "SOSALERT", SOSClosureNotes, CompanyId, CurrentUserId);
            }

            if (CloseSOS && !CloseAllSOS)
            {
                var sos = await  _context.Set<Sosalert>().Where(SA=> SA.SosalertId == SOSAlertID && SA.Completed == false).FirstOrDefaultAsync();
                if (sos != null)
                {
                    sos.Completed = true;
                    sos.CompletedBy = CurrentUserId;
                    sos.CompletedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    _context.Update(sos);
                   await  _context.SaveChangesAsync();
                }
            }
            else if (CloseSOS && CloseAllSOS)
            {
                var soslist = await _context.Set<Sosalert>().Where(SA => SA.UserId == UserID && SA.Completed == false).ToListAsync();
                foreach (var sos in soslist)
                {
                    sos.Completed = true;
                    sos.CompletedBy = CurrentUserId;
                    sos.CompletedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    _context.Update(sos);
                }
               
                await _context.SaveChangesAsync();
            }

            if (CloseSOSIncident && ActiveIncidentID > 0)
            {
              await  UpdateIncidentStatus(CompanyId, ActiveIncidentID, "CLOSE", TimeZoneId, CurrentUserId, "ADMIN", "SOS Closed", 1, SOSClosureNotes, isSos: true);
            }

            if (CloseSOS == true && CaseNoteIDs.Length > 0)
            {
                foreach (int AlertID in CaseNoteIDs)
                {
                    var sos = await  _context.Set<Sosalert>().Where(SA=> SA.SosalertId == AlertID).FirstOrDefaultAsync();
                    if (sos != null)
                    {
                        sos.Completed = true;
                        sos.CompletedBy = CurrentUserId;
                        sos.CompletedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        _context.Update(sos);
                        await _context.SaveChangesAsync();
                        if (sos.AlertType == "SOS")
                        {
                          await  UpdateIncidentStatus(CompanyId, (int)sos.ActiveIncidentId, "CLOSE", TimeZoneId, CurrentUserId, "ADMIN", "SOS Closed", 1, SOSClosureNotes, isSos: true);
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            
            return false;
        }
    }
    public async Task<int> CheckUserSOS(int ActiveIncidentID, int UserID)
    {
        try
        {
            int sosuser = await  _context.Set<Sosalert>()
                           .Where(SA=> SA.ActiveIncidentId != ActiveIncidentID && SA.UserId == UserID
                           && SA.Completed == false).CountAsync();
            return sosuser;
        }
        catch (Exception ex)
        {
            throw ex;
            
        }
    }
    public async Task<UpdateIncidentStatusReturn> UpdateIncidentStatus(int CompanyId, int IncidentActivationId, string Type, string TimeZoneId, int CurrentUserId,
            string UserRole, string Reason, int NumberOfKeyHolder, string CompletionNotes = "", int[] MessageMethod = null, int CascadePlanID = 0, bool isSos = false)
    {
        var Incidt = (from inc in _context.Set<IncidentActivation>()
                      join L in _context.Set<Location>() on inc.ImpactedLocationId equals L.LocationId
                      where inc.CompanyId == CompanyId && inc.IncidentActivationId == IncidentActivationId
                      select new { inc, L.LocationName }).FirstOrDefault();

        DateTimeOffset LocalTime = DateTime.Now.GetDateTimeOffset(TimeZoneId);
        if (Incidt != null)
        {

            if (Type.ToUpper() == "DEACTIVATE" && Incidt.inc.Status != 3)
            {
                Incidt.inc.Status = 3;
                Incidt.inc.DeactivatedOn = LocalTime;
                Incidt.inc.DeactivatedBy = CurrentUserId;
            }
            else if (Type.ToUpper() == "CLOSE" && Incidt.inc.Status != 4)
            {
                if (Incidt.inc.DeactivatedBy == 0)
                {
                    Incidt.inc.DeactivatedOn = LocalTime;
                    Incidt.inc.DeactivatedBy = CurrentUserId;
                }
                Incidt.inc.Status = 4;
                Incidt.inc.ClosedOn = LocalTime;
                Incidt.inc.ClosedBy = CurrentUserId;

                //Add Completion Notes
                if (!string.IsNullOrEmpty(CompletionNotes))
                {
                    Incidt.inc.HasNotes = true;
                    await IncidentNote(Incidt.inc.IncidentActivationId, "INCIDENT", CompletionNotes, CompanyId, CurrentUserId);
                }

               
              await  _activeIncidentRepository.CompleteAllTask(Incidt.inc.IncidentActivationId, CurrentUserId, CompanyId, TimeZoneId);

            }
            else if (Type.ToUpper() == "CANCEL" && Incidt.inc.Status != 5)
            {
                Incidt.inc.Status = 5;
                Incidt.inc.ClosedOn = LocalTime;
                Incidt.inc.ClosedBy = CurrentUserId;
            }
            else
            {
                var rtninci = await _context.Set<IncidentActivation>()
                               .Where(Incidentval=> Incidentval.CompanyId == CompanyId && Incidentval.IncidentActivationId == IncidentActivationId)
                               .Select(Incidentval=> new UpdateIncidentStatusReturn { IncidentActivationId = Incidentval.IncidentActivationId }).FirstOrDefaultAsync();

                return rtninci;
            }
            Incidt.inc.UpdatedBy = CurrentUserId;
            Incidt.inc.UpdatedOn = LocalTime;
            _context.Update(Incidt);
            await _context.SaveChangesAsync();

            IncidentDeActivationReason tblIncidentDeactiReason = new IncidentDeActivationReason()
            {
                CompanyId = CompanyId,
                Reason = !string.IsNullOrEmpty(Reason) ? Reason.Trim() : "",
                Type = Type,
                IncidentActivationId = IncidentActivationId,
                CreatedOn = LocalTime,
                CreatedBy = CurrentUserId,
                UpdatedOn = LocalTime,
                UpdatedBy = CurrentUserId,
            };
           await _context.AddAsync(tblIncidentDeactiReason);
           await _context.SaveChangesAsync();
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            //string sendClosureAlert = "true";
            //sendClosureAlert = DBC.GetCompanyParameter("SEND_INCIDENT_CLOSURE_PING", CompanyId);

            if (Type.ToUpper() == "CLOSE")
            {

                // This will send a clouser message as Ping
                UserFullName closedUser = await _context.Set<User>().Where(U=> U.UserId == Incidt.inc.ClosedBy).Select(s => new UserFullName { Firstname = s.FirstName, Lastname = s.LastName }).SingleOrDefaultAsync();
                string MsgText = Incidt.inc.Name + " at " + Incidt.LocationName + ", launched on " + Incidt.inc.LaunchedOn.ToString("dd-MMM-yyyy HH:mm") + " is closed by " + DBC.UserName(closedUser) + " with the following reason:" + Environment.NewLine + "\"" + Reason + "\"";

                Messaging MSG = new Messaging(_context, _httpContextAccessor);
                MSG.TimeZoneId = TimeZoneId;
                MSG.CascadePlanID = CascadePlanID;
                MSG.MessageSourceAction = isSos ? SourceAction.SosClosure : SourceAction.IncidentClosure;
                int tblmessageid =await MSG.CreateMessage(CompanyId, MsgText, "Ping", IncidentActivationId, 500, CurrentUserId, 1, LocalTime, false, null, 99,
                    0, 0, false, false, MessageMethod);

                var pMessageId = new SqlParameter("@MessageID", tblmessageid);
                var pIncidentActivationId = new SqlParameter("@IncidentActivationID", IncidentActivationId);
                var pCustomerTime = new SqlParameter("@CustomerTime", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                try
                {
                    int RowsCount =await _context.Database.ExecuteSqlRawAsync("Pro_Incident_Closure_Message_List @IncidentActivationID,@MessageID,@CustomerTime",
                       pIncidentActivationId, pMessageId, pCustomerTime);

                    IsFundAvailable = await MSG.CalculateMessageCost(CompanyId, tblmessageid, MsgText);

                    Task.Factory.StartNew(() => QueueHelper.MessageDeviceQueue(tblmessageid, "Ping", 1, CascadePlanID));

                    //QueueHelper.MessageDevicePublish(tblmessageid, 1);
                    QueueConsumer.CreateCascadingJobs(CascadePlanID, tblmessageid, 0, CompanyId, TimeZoneId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                MSG.DeleteMessageMethod(0, IncidentActivationId);
                DBC.CancelJobsByGroup("MESSAGE_CASCADE_" + IncidentActivationId);
            }

            SendEmail SDE = new SendEmail(_context,DBC);
            var roles = DBC.CCRoles();
            if (Type.ToUpper() == "DEACTIVATE")
            {
                if (!roles.Contains(UserRole.ToUpper()) && NumberOfKeyHolder > 1)
                {
                    SDE.NotifyKeyHolders("deactivate", IncidentActivationId, CurrentUserId, CompanyId, Reason);
                }
            }
            else if (Type.ToUpper() == "CANCEL")
            {
                if (NumberOfKeyHolder > 1)
                {
                    SDE.NotifyKeyHolders("cancel", IncidentActivationId, CurrentUserId, CompanyId, Reason);
                }
            }

            var inciList = _context.Set<IncidentActivation>()
                            .Where(Incidentval=> Incidentval.CompanyId == CompanyId && Incidentval.IncidentActivationId == IncidentActivationId)
                            .Select(Incidentval=> new UpdateIncidentStatusReturn { IncidentActivationId = Incidentval.IncidentActivationId }).FirstOrDefault();

            return inciList;
        }
        else
        {
            return null;
        }
    }
    public async Task<List<IncidentSOSRequest>> GetIncidentSOSRequest(int IncidentActivationId)
    {
        try
        {
            var pIncidentActivationId = new SqlParameter("@IncidentActivationID", IncidentActivationId);
            var sosuser = await _context.Set<IncidentSOSRequest>().FromSqlRaw("exec Pro_ActiveIncident_GetIncidentSOSRequest @IncidentActivationID", pIncidentActivationId).ToListAsync();
            return sosuser;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<UpdateIncidentStatusReturn> GetActiveIncidentBasic(int CompanyId, int IncidentActivationId)
    {
        try
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            var incidentStatus= await _context.Set<UpdateIncidentStatusReturn>().FromSqlRaw("exec Pro_Get_Active_Incident_Basic @CompanyID, @IncidentActivationID", CompanyId, IncidentActivationId).FirstOrDefaultAsync();
            incidentStatus.Status = DBC.LookupWithKey("IncidentStatus");

            return incidentStatus;
        }
        catch (Exception ex)
        {
            throw ex;
            return new UpdateIncidentStatusReturn();
        }
             
    }
    public async Task<int> UpdateIncidentType(string Name, int IncidentTypeId, int UserId, int CompanyId)
    {
        try
        {
            if (IncidentTypeId > 0)
            {
                var IncidentTypeExist = await _context.Set<IncidentType>().Where(LIT=> LIT.IncidentTypeId == IncidentTypeId).FirstOrDefaultAsync();
                if (IncidentTypeExist != null)
                {
                    IncidentTypeExist.Name = Name;
                    _context.Update(IncidentTypeExist);
                    await _context.SaveChangesAsync();
                    return IncidentTypeId;
                }
            }
            else
            {
                var IncidentTypeExist = await _context.Set<IncidentType>().Where(LIT=> LIT.Name == Name && LIT.CompanyId == CompanyId).FirstOrDefaultAsync();
                if (IncidentTypeExist != null)
                {
                    IncidentTypeExist.Status = 1;
                    IncidentTypeExist.CompanyId = CompanyId;
                    _context.Update(IncidentTypeExist);
                    await _context.SaveChangesAsync();
                    return IncidentTypeExist.IncidentTypeId;
                }
                else
                {
                    IncidentType newIncidentType = new IncidentType()
                    {
                        Name = Name,
                        CompanyId = CompanyId,
                        Status = 1
                    };
                    await _context.AddAsync(newIncidentType);
                    await _context.SaveChangesAsync();
                    return newIncidentType.IncidentTypeId;
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<ActionLsts>> AddIncidentActions(int CompanyId, int IncidentId, string Title, string ActionDescription,
           IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify, int Status, int CurrentUserId, string TimeZoneId)
    {
        if (ActionDescription != "")
        {
            IncidentAction tblIncidentAction = new IncidentAction()
            {
                IncidentId = IncidentId,
                Title = Title,
                ActionDescription = ActionDescription,
                CompanyId = CompanyId,
                Status = Status,
                CreatedBy = CurrentUserId,
                CreatedOn = DateTime.Now,
                UpdatedBy = CurrentUserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
            };
            await _context.AddAsync(tblIncidentAction);
            await _context.SaveChangesAsync();

            await IncidentParticipantGroup(IncidentId, tblIncidentAction.IncidentActionId, IncidentParticipants);
            await IncidentParticipantUser(IncidentId, tblIncidentAction.IncidentActionId, UsersToNotify);
        }

        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
        var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
        return await _context.Set<ActionLsts>().FromSqlRaw(
                "Exec Pro_Incident_GetIncidentByRef_ActionList @CompanyID,@IncidentID", pCompanyID, pIncidentID).ToListAsync();

    }
    public async Task IncidentParticipantGroup(int IncidentId, int ActionID, IncidentNotificationObjLst[] IncidentParticipants)
    {
        try
        {

            var ExsGroupList = await  _context.Set<IncidentParticipant>()
                                .Where(IP=> IP.IncidentId == IncidentId && IP.ParticipantType == "GROUP"
                               ).ToListAsync();

            if (ActionID > 0)
            {
                ExsGroupList = ExsGroupList.Where(s => s.IncidentActionId == ActionID).ToList();
            }
            else
            {
                ExsGroupList = ExsGroupList.Where(s => s.IncidentActionId == 0).ToList();
            }

            List<int[]> ObjList = new List<int[]>();

            if (IncidentParticipants.Length > 0)
            {
                foreach (IncidentNotificationObjLst group in IncidentParticipants)
                {
                    int groupid = group.SourceObjectPrimaryId;
                    int objmapid = group.ObjectMappingId;

                    var ISExist = ExsGroupList.FirstOrDefault(s => ((s.IncidentId == IncidentId && ActionID == 0) || (s.IncidentActionId == ActionID && ActionID > 0)) &&
                    s.ObjectMappingId == objmapid && s.ParticipantGroupId == groupid);

                    if (ISExist == null)
                    {
                       await CreateIncidentParticipant(IncidentId, groupid, objmapid, ActionID, 0, "GROUP");
                    }
                    else
                    {
                        int[] Arr = new int[4];
                        Arr[0] = ISExist.IncidentId;
                        Arr[1] = ISExist.ObjectMappingId;
                        Arr[2] = (int)ISExist.ParticipantGroupId;
                        Arr[3] = (int)ISExist.IncidentActionId;
                        ObjList.Add(Arr);
                    }
                }
            }

            foreach (var item in ExsGroupList)
            {
                bool ISDEL = true;
                if (ActionID > 0)
                {
                    ISDEL = ObjList.Any(s => s[0] == item.IncidentId && s[1] == item.ObjectMappingId &&
                        s[2] == item.ParticipantGroupId && item.ParticipantType == "GROUP" && s[3] == item.IncidentActionId);
                }
                else
                {
                    ISDEL = ObjList.Any(s => s[0] == item.IncidentId && s[1] == item.ObjectMappingId &&
                        s[2] == item.ParticipantGroupId && item.ParticipantType == "GROUP");
                }
                if (!ISDEL)
                {
                    _context.Remove(item);
                }
            }
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task IncidentParticipantUser(int IncidentId, int ActionID, int[] UsersToNotify)
    {
        try
        {

            var ExsUserList = await  _context.Set<IncidentParticipant>()
                               .Where(IP=> IP.IncidentId == IncidentId && IP.ParticipantType == "USER").ToListAsync();

            if (ActionID > 0)
            {
                ExsUserList = ExsUserList.Where(s => s.IncidentActionId == ActionID).ToList();
            }
            else
            {
                ExsUserList = ExsUserList.Where(s => s.IncidentActionId == 0).ToList();
            }


            List<int[]> ObjList = new List<int[]>();

            if (UsersToNotify.Length > 0)
            {
                foreach (int userid in UsersToNotify)
                {
                    var ISExist = ExsUserList.FirstOrDefault(s => ((s.IncidentId == IncidentId && ActionID == 0) || (s.IncidentActionId == ActionID && ActionID > 0))
                    && s.ParticipantUserId == userid);

                    if (ISExist == null)
                    {
                        await CreateIncidentParticipant(IncidentId, 0, 0, ActionID, userid, "USER");
                    }
                    else
                    {
                        int[] Arr = new int[3];
                        Arr[0] = ISExist.IncidentId;
                        Arr[1] = (int)ISExist.ParticipantUserId;
                        Arr[2] = (int)ISExist.IncidentActionId;
                        ObjList.Add(Arr);
                    }
                }
            }

            foreach (var item in ExsUserList)
            {
                bool ISDEL = true;
                if (ActionID > 0)
                {
                    ISDEL = ObjList.Any(s => s[0] == item.IncidentId && s[1] == item.ParticipantUserId && item.ParticipantType == "USER" && s[2] == item.IncidentActionId);
                }
                else
                {
                    ISDEL = ObjList.Any(s => s[0] == item.IncidentId && s[1] == item.ParticipantUserId && item.ParticipantType == "USER");
                }
                if (!ISDEL)
                {
                    _context.Remove(item);
                }
            }
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task CreateIncidentParticipant(int IncidentID, int GroupID, int ObjectMapID, int IncidentActionID, int UserID, string ParticipantType)
    {
        try
        {
            IncidentParticipant IP = new IncidentParticipant();
            IP.IncidentId = IncidentID;
            IP.ParticipantGroupId = GroupID;
            IP.ObjectMappingId = ObjectMapID;
            IP.IncidentActionId = IncidentActionID;
            IP.ParticipantUserId = UserID;
            IP.ParticipantType = ParticipantType;
            await _context.AddAsync(IP);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TourIncident> GetIncidentByName(string IncidentName, int CompanyId, int UserId, string TimeZoneId)
    {
        try
        {
            var pIncidentName = new SqlParameter("@IncidentName", Uri.UnescapeDataString(IncidentName));
            var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
            var pUserId = new SqlParameter("@UserID", UserId);
            var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset( TimeZoneId));

            var result = await _context.Set<TourIncident>().FromSqlRaw("exec Pro_Get_Incident_ByName @IncidentName, @CompanyID, @UserID, @CustomerTime",
                pIncidentName, pCompanyId, pUserId, pCustomerTime).FirstOrDefaultAsync();
            if (result != null)
            {
                return result;
            }
            else
            {
                return new TourIncident();
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<IncidentAssets>> AddIncidentAssets(int CompanyId, int IncidentId, string LinkedAssetId, int CurrentUserId, string TimeZoneId)
    {
        string[] SubFilter = LinkedAssetId.Split(',');
        if (SubFilter.Length > 0)
        {
            int delObjMapId = Convert.ToInt16(SubFilter[0]);

            var QueryRec = await _context.Set<ObjectRelation>()
                            .Where(Userdelval=> Userdelval.TargetObjectPrimaryId == IncidentId && Userdelval.ObjectMappingId == delObjMapId
                            ).ToListAsync();

            List<int> OBRIDList = new List<int>();
            for (int loop = 0; loop < SubFilter.Length; loop++)
            {
                if (loop == 0)
                {
                    //Do nothing
                }
                else
                {
                    int objmapid = Convert.ToInt32(SubFilter[0]);
                    int SOPId = Convert.ToInt32(SubFilter[loop]);
                    var ISExist = QueryRec.FirstOrDefault(s => s.TargetObjectPrimaryId == IncidentId && s.ObjectMappingId == objmapid && s.SourceObjectPrimaryId == SOPId);

                    if (ISExist == null)
                    {
                        ObjectRelation tblObjRel = new ObjectRelation()
                        {
                            TargetObjectPrimaryId = IncidentId,
                            ObjectMappingId = Convert.ToInt32(SubFilter[0]),
                            SourceObjectPrimaryId = Convert.ToInt32(SubFilter[loop]),
                            CreatedBy = CurrentUserId,
                            UpdatedBy = CurrentUserId,
                            CreatedOn = System.DateTime.Now,
                            UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId)
                        };
                        await _context.AddAsync(tblObjRel);
                    }
                    else
                    {
                        OBRIDList.Add(ISExist.ObjectRelationId);
                    }
                }
            }
            foreach (var Ditem in QueryRec)
            {
                bool ISDEL = OBRIDList.Any(s => s == Ditem.ObjectRelationId);
                if (!ISDEL)
                {
                    _context.Remove(Ditem);
                }
            }
            await _context.SaveChangesAsync();
        }
        var pCompanyID = new SqlParameter("@CompanyId" , CompanyId);
        var pIncidentID = new SqlParameter("@IncidentId", IncidentId);
        return await _context.Set<IncidentAssets>().FromSqlRaw("exec Pro_Get_Incident_Assets @CompanyId, @IncidentId", pCompanyID, pIncidentID).ToListAsync();
    }

    public async Task<int> UpdateCompanyIncidents(int CompanyId, int IncidentId, string IncidentIcon, string Name, string Description,
      int PlanAssetID, int IncidentTypeId, int Severity, int Status, int NumberOfKeyHolders, int CurrentUserId, string TimeZoneId,
      UpdIncidentKeyHldLst[] UpdIncidentKeyHldLst, int AudioAssetId, bool TrackUser, bool SilentMessage, List<AckOption> AckOptions,
      int[] MessageMethod, int CascadePlanID, int[] Groups, int[] Keyholders)
    {
        DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
        var Incidt = await _context.Set<Incident>()
                      .Where(inc=> inc.CompanyId == CompanyId && inc.IncidentId == IncidentId).FirstOrDefaultAsync();

        if (Incidt != null)
        {
            string allow_nominated_kh = DBC.GetCompanyParameter("ALLOW_KEYHOLDER_NOMINATION", CompanyId);
            Incidt.IncidentIcon = IncidentIcon;
            Incidt.Name = Name;
            Incidt.Description = Description;
            Incidt.PlanAssetId = PlanAssetID;
            Incidt.IncidentTypeId = IncidentTypeId;
            Incidt.Severity = Severity;
            Incidt.Status = Status;
            Incidt.AudioAssetId = AudioAssetId;
            Incidt.TrackUser = TrackUser;
            Incidt.SilentMessage = SilentMessage;
            Incidt.NumberOfKeyHolders = NumberOfKeyHolders;
            Incidt.CascadePlanId = CascadePlanID;
            Incidt.UpdatedBy = CurrentUserId;
            Incidt.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
            Incidt.HasNominatedKeyholders = (Keyholders.Length > 0 && NumberOfKeyHolders > 1 && allow_nominated_kh == "true");
            await _context.SaveChangesAsync();
        }

        await ProcessKeyContacts(CompanyId, IncidentId, CurrentUserId, TimeZoneId, UpdIncidentKeyHldLst);

        await ProcessKeyholders(CompanyId, IncidentId, CurrentUserId, Keyholders);

        if (AckOptions != null)
        {
            await SaveIncidentMessageResponse(AckOptions, IncidentId);
        }

        if (Groups != null)
        {
            await AddIncidentGroup(IncidentId, Groups, CompanyId);
        }

        if (MessageMethod != null)
        {
            if (MessageMethod.Length > 0)
            {
                var items = await _context.Set<MessageMethod>().Where(MM=> MM.IncidentId == IncidentId).ToListAsync();
                _context.RemoveRange(items);
               await _context.SaveChangesAsync();

                if (CascadePlanID <= 0)
                {
                    Messaging MSG = new Messaging(_context,_httpContextAccessor);
                    foreach (int Method in MessageMethod)
                    {
                       await MSG.CreateMessageMethod(0, Method, 0, IncidentId);
                    }
                }
            }
        }

        await DeleteEmptyTaskHeader(IncidentId);

        return Incidt.IncidentId;
    }

    private async Task ProcessKeyContacts(int CompanyId, int IncidentId, int CurrentUserId, string TimeZoneId, UpdIncidentKeyHldLst[] UpdIncidentKeyHldLst)
    {
        var incKeyHlds =  await _context.Set<IncidentKeyContact>()
                          .Where(incKey=> incKey.CompanyId == CompanyId && incKey.IncidentId == IncidentId && (incKey.IncidentActionId == null ||
                          incKey.IncidentActionId == 0)
                          ).ToListAsync();

        List<int> KEYLIST = new List<int>();

        foreach (var IKeyHld in UpdIncidentKeyHldLst)
        {
            if (IKeyHld.UserID != null)
            {
                var ISExist = incKeyHlds.FirstOrDefault(s => s.CompanyId == CompanyId && s.IncidentId == IncidentId && s.UserId == IKeyHld.UserID.Value);
                if (ISExist == null)
                {
                    IncidentKeyContact tblIncKeyContact = new IncidentKeyContact()
                    {
                        CompanyId = CompanyId,
                        IncidentId = IncidentId,
                        UserId = IKeyHld.UserID.Value,
                        CreatedBy = CurrentUserId,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = CurrentUserId,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                    };
                    await _context.AddAsync(tblIncKeyContact);
                }
                else
                {
                    KEYLIST.Add(ISExist.IncidentKeyContactId);
                }
            }
        }
        foreach (var Ditem in incKeyHlds)
        {
            bool ISDEL = KEYLIST.Any(s => s == Ditem.IncidentKeyContactId);
            if (!ISDEL)
            {
                _context.Remove(Ditem);
            }
        }
        await _context.SaveChangesAsync();
    }
    public async Task DeleteEmptyTaskHeader(int IncidentID)
    {
        try
        {

            var header = await _context.Set<TaskHeader>().Where(TH=> TH.IncidentId == IncidentID).FirstOrDefaultAsync();

            int TaskList = await _context.Set<TaskIncident>()
                            .Where(TL=> TL.IncidentId == IncidentID && TL.TaskHeaderId == header.TaskHeaderId).CountAsync();

            if (header != null && TaskList == 0)
            {
                var incident = await _context.Set<Incident>().Where(I=> I.IncidentId == header.IncidentId).FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.HasTask = false;
                }

                _context.Remove(header);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {

        }
    }
    public async Task<List<ActionLsts>> UpdateCompanyIncidentActions(int CompanyId, int IncidentId, int IncidentActionId, string Title, string ActionDescription,
           IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify, int Status, int CurrentUserId, string TimeZoneId)
    {
        var IncidtAct = await _context.Set<IncidentAction>()
                         .Where(inc=> inc.CompanyId == CompanyId && inc.IncidentActionId == IncidentActionId && inc.IncidentId == IncidentId).FirstOrDefaultAsync();

        if (IncidtAct != null)
        {
            IncidtAct.IncidentActionId = IncidentActionId;
            IncidtAct.IncidentId = IncidentId;
            IncidtAct.Title = Title;
            IncidtAct.ActionDescription = ActionDescription;
            IncidtAct.Status = Status;
            IncidtAct.UpdatedBy = CurrentUserId;
            IncidtAct.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            await _context.SaveChangesAsync();

            await IncidentParticipantGroup(IncidentId, IncidentActionId, IncidentParticipants);
            await IncidentParticipantUser(IncidentId, IncidentActionId, UsersToNotify);
        }

        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
        var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
        return await _context.Set<ActionLsts>().FromSqlRaw(
                "exec Pro_Incident_GetIncidentByRef_ActionList @CompanyID,@IncidentID", pCompanyID, pIncidentID).ToListAsync();

    }

    public async Task<bool> DeleteCompanyIncidents(int CompanyId, int IncidentId, int CurrentUserId, string TimeZoneId)
    {
        var Incidt =  await _context.Set<Incident>()
                      .Where(inc=> inc.CompanyId == CompanyId && inc.IncidentId == IncidentId).FirstOrDefaultAsync();

        if (Incidt != null)
        {
            Incidt.Status = 3;
            Incidt.UpdatedBy = CurrentUserId;
            Incidt.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            _context.Update(Incidt);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteCompanyIncidentActions(int CompanyId, int IncidentId, int IncidentActionId, int CurrentUserId, string TimeZoneId)
    {
        var IncidtAct = await _context.Set<IncidentAction>()
                         .Where(inc=> inc.CompanyId == CompanyId && inc.IncidentActionId == IncidentActionId && inc.IncidentId == IncidentId).FirstOrDefaultAsync();

        if (IncidtAct != null)
        {
            IncidtAct.Status = 3;
            IncidtAct.UpdatedBy = CurrentUserId;
            IncidtAct.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
            _context.Update(IncidtAct);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteIncidentAssets(int CompanyId, int IncidentId, int AssetObjMapId, int IncidentAssetId)
    {
        bool IsDeleted = false;
        var QueryRec = (from OJR in _context.Set<ObjectRelation>()
                        let ObjMap = from OM in _context.Set<ObjectMapping>()
                                     join OS in _context.Set<Core.Models.Object>() on OM.SourceObjectId equals OS.ObjectId
                                     join OT in _context.Set<Core.Models.Object>() on OM.TargetObjectId equals OT.ObjectId
                                     where OS.ObjectName == "AssetDetails" && OT.ObjectName == "IncidentDetails"
                                     select OM.ObjectMappingId
                        join AST in _context.Set<Assets>() on OJR.SourceObjectPrimaryId equals AST.AssetId
                        where ObjMap.Contains(OJR.ObjectMappingId) && AST.CompanyId == CompanyId
                        && OJR.TargetObjectPrimaryId == IncidentId
                        && OJR.ObjectMappingId == AssetObjMapId && OJR.SourceObjectPrimaryId == IncidentAssetId
                        select OJR).FirstOrDefault();

        if (QueryRec != null)
        { //Do not delete all Action

            _context.Remove(QueryRec);
            await _context.SaveChangesAsync();
            IsDeleted = true;
        }
        return IsDeleted;
    }
    public async Task<UpdateIncidentStatus> ActiveIncidentDetailsById(int CompanyId, int IncidentActivationId, int CurrentUserID, bool isapp = false)
    {
        try
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            string webpath = DBC.LookupWithKey("PORTAL");
            //string nophoto = webpath + "/uploads/userphoto/no-photo.jpg";
            string company_path = CompanyId.ToString();

            bool ShowTrackUser = false;
            bool ShowSilentMessage = false;
            bool ShowMessageMethod = false;

            bool.TryParse(DBC.GetCompanyParameter("SHOW_TRACK_USER_FIELD", CompanyId), out ShowTrackUser);
            bool.TryParse(DBC.GetCompanyParameter("SHOW_SILENT_MESSAGE_FIELD", CompanyId), out ShowSilentMessage);
            bool.TryParse(DBC.GetCompanyParameter("SHOW_MESSAGE_METHOD_FIELD", CompanyId), out ShowMessageMethod);

            //bool iskc = (from AKC in db.ActiveIncidentKeyContact where AKC.UserId == CurrentUserID && AKC.IncidentActivationId == IncidentActivationId select AKC).Any();
            var pCompanyID = new SqlParameter("CompanyID", CompanyId);
            var pCurrentUserID = new SqlParameter("CurrentUserId", CurrentUserID);
            var pIncidentActivationID = new SqlParameter("IncidentActivationID", IncidentActivationId);
            var activeincident = await _context.Set<UpdateIncidentStatus>().FromSqlRaw("exec Pro_Active_Incident_Details_By_Id @CompanyID, @IncidentActivationID, @CurrentUserID", pCompanyID, pIncidentActivationID, pCurrentUserID).FirstOrDefaultAsync();
            activeincident.AckOptions = await _context.Set<ActiveMessageResponse>()
                                                    .Where(MM => MM.ActiveIncidentId == activeincident.IncidentActivationId)
                                                    .Select(MM => new AckOption
                                                    {
                                                        ResponseId = MM.ResponseId,
                                                        ResponseLabel = MM.ResponseLabel,
                                                        ResponseCode = MM.ResponseCode
                                                    }).ToListAsync();
            activeincident.MessageMethod =await _context.Set<Core.Messages.MessageMethod>().Include(x => x.CommsMethod)
                                         .Where(MM => MM.ActiveIncidentId == activeincident.IncidentActivationId)
                                         .Select(MM => new CommsMethods { MethodId = MM.MethodId, MethodName = MM.CommsMethod.MethodName }).ToListAsync();
            activeincident.Status = DBC.LookupWithKey("IncidentStatus");
            var pIncidentActivationID2 = new SqlParameter("IncidentActivationID", activeincident.IncidentActivationId);
            activeincident.IncNotificationLst = await _context.Set<IIncNotificationLst>().FromSqlRaw("exec Pro_Get_IIncNotificationLst @CompanyID,@IncidentActivationID", pCompanyID, pIncidentActivationID2).ToListAsync();
                       activeincident.ActionLst = await _context.Set<IncidentAction>()
                                                   .Where(IncidentAct => IncidentAct.IncidentId == activeincident.IncidentId)
                                                   .Select(IncidentAct => new ActionLsts
                                                   {
                                                       IncidentActionId = IncidentAct.IncidentActionId,
                                                       IncidentId = IncidentAct.IncidentId,
                                                       Title = IncidentAct.Title,
                                                       ActionDescription = IncidentAct.ActionDescription,
                                                       MultiResponse = 0,
                                                       Status = IncidentAct.Status,
                                                       CompanyId = IncidentAct.CompanyId,
                                                       HasParticipants = true
                                                   }).ToListAsync();
            activeincident.AffectedLocations = (from IL in _context.Set<IncidentLocation>()
                                                join ILL in _context.Set<IncidentLocationLibrary>() on IL.LibLocationId equals ILL.LocationId
                                                where IL.IncidentActivationId == activeincident.IncidentActivationId
                                                select new AffectedLocation
                                                {
                                                    LocationName = ILL.LocationName,
                                                    Lat = ILL.Lat,
                                                    Lng = ILL.Lng,
                                                    Address = ILL.Address,
                                                    LocationType = ILL.LocationType,
                                                    ImpactedLocationID = (int)IL.ImpactedLocationId,
                                                    LocationID = ILL.LocationId
                                                }).ToList();
                                

            activeincident.SegregationWarning = await DBC.SegregationWarning(CompanyId, CurrentUserID, activeincident.IncidentId);

            var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentActivationId = new SqlParameter("@IncidentActivationId", IncidentActivationId);
            var pIsApp = new SqlParameter("@IsApp", isapp);
            activeincident.IncKeyCon = await _context.Set<IIncKeyConResponse>().FromSqlRaw("exec Pro_ActiveIncident_GetIncidentKeyContacts @CompanyID, @IncidentActivationId, @IsApp", pCompanyId, pIncidentActivationId, pIsApp).ToListAsync();

            var pIncidentActivationId2 = new SqlParameter("@IncidentActivationId", IncidentActivationId);
            activeincident.UsersToNotify = await _context.Set<IIncKeyConResponse>().FromSqlRaw("exec Pro_ActiveIncident_GetUsersToNotify @IncidentActivationId", pIncidentActivationId2).ToListAsync();

            var pCompanyID3 = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID3 = new SqlParameter("@IncidentID", activeincident.IncidentId);
            var pIncidentActionID3 = new SqlParameter("@IncidentActionID", -1);
            activeincident.Participants =await _context.Set<IncidentParticipants>().FromSqlRaw("exec Pro_Incident_GetIncidentByRef_Participant @CompanyID, @IncidentID, @IncidentActionID",
                pCompanyID3, pIncidentID3, pIncidentActionID3).ToListAsync();

            if (string.IsNullOrEmpty(activeincident.SocialHandle))
            {
                activeincident.SocialHandles = new List<SocialHandles>();
            }
            else
            {
                var socialprovider =await DBC.GetSocialServiceProviders();
                List<string> selectedpro = activeincident.SocialHandle.Split(',').ToList();
                activeincident.SocialHandles = socialprovider.Where(w => selectedpro.Contains(w.ProviderCode)).ToList();
            }

            return activeincident;

        }
       
        catch (Exception ex)
        {
            
            return new UpdateIncidentStatus();
        }


    }
    public async Task<List<IncidentLibraryDetails>> IncidentLibrary(int CompanyId)
    {
        try
        {
            //Use: EXEC [dbo].[Pro_Incident_GetIncidentLibraries] @CompanyID
            var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
            var incidents =await _context.Set<IncidentLibraryDetails>().FromSqlRaw(
                "exec Pro_Incident_GetIncidentLibraries @CompanyID",
                pCompanyId).ToListAsync();

            return incidents;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<int> CopyIncident(int CompanyId, int LibIncidentId, int OutUserCompanyId, int OutLoginUserId, int CurrentUserId, string TimeZoneId)
    {
        DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
        var IncidentDtl = await _context.Set<LibIncident>()
                           .Where(LI=> LI.LibIncidentId == LibIncidentId)
                           .FirstOrDefaultAsync();

        int RetIncidentId = 0;

        var IncidentType =  await _context.Set<LibIncidentType>().Where(LIT=> LIT.LibIncidentTypeId == IncidentDtl.LibIncidentTypeId).FirstOrDefaultAsync();
        int TypeId = 0;
        if (IncidentDtl != null)
        {
            var TypeCheck = await _context.Set<IncidentType>().Where(IT=> IT.CompanyId == OutUserCompanyId && IT.Name == IncidentType.Name).FirstOrDefaultAsync();
            if (TypeCheck != null)
            {
                TypeId = TypeCheck.IncidentTypeId;
            }
            else
            {
                IncidentType newIncidentType = new IncidentType()
                {
                    CompanyId = OutUserCompanyId,
                    Name = IncidentType.Name,
                    Status = 1
                };
                await _context.AddAsync(newIncidentType);
                await _context.SaveChangesAsync();
                TypeId = newIncidentType.IncidentTypeId;
            }

            Incident newIncident = new Incident()
            {
                CompanyId = CompanyId,
                Name = IncidentDtl.Name,
                Description = IncidentDtl.Description,
                IncidentTypeId = TypeId,
                IncidentIcon = IncidentDtl.LibIncodentIcon,
                Status = 0,
                Severity = IncidentDtl.Severity,
                IsSos = IncidentDtl.IsSos,
                HasTask = false,
                AudioAssetId = 0,
                SilentMessage = false,
                TrackUser = false,
                CreatedBy = CurrentUserId,
                UpdatedBy = CurrentUserId,
                CreatedOn = System.DateTime.Now,
                UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
            };
            await _context.AddAsync(newIncident);
            await _context.SaveChangesAsync();
            RetIncidentId = newIncident.IncidentId;
        }
        return RetIncidentId;
    }
    public async Task<List<ActionLsts>> IncidentMessage(int CompanyId, int IncidentId)
    {
        try
        {
            //Use: EXEC [dbo].[Pro_Incident_GetIncidentByRef_ActionList] @CompanyID,@IncidentID
            var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
            return await _context.Set<ActionLsts>().FromSqlRaw(
                "exec Pro_Incident_GetIncidentByRef_ActionList @CompanyID,@IncidentID",
                pCompanyID,
                pIncidentID).ToListAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<ActionLsts> GetIncidentAction(int CompanyId, int IncidentId, int IncidentActionId)
    {
       
        try
        {
            //Use: EXEC [dbo].[Pro_Incident_GetIncidentAction] @CompanyID, @IncidentId, @IncidentActionId
            var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
            var pIncidentID = new SqlParameter("@IncidentId", IncidentId);
            var pIncidentActionId = new SqlParameter("@IncidentActionId", IncidentActionId);
            var result =await _context.Set<ActionLsts>().FromSqlRaw(
                "exec Pro_Incident_GetIncidentAction @CompanyID, @IncidentId, @IncidentActionId",
                pCompanyID,
                pIncidentID,
                pIncidentActionId).FirstOrDefaultAsync();

            if (result != null)
            {
                var pCompanyID2 = new SqlParameter("@CompanyID", CompanyId);
                var pIncidentID2 = new SqlParameter("@IncidentID", IncidentId);
                var pIncidentActionId2 = new SqlParameter("@IncidentActionID", IncidentActionId);

                result.Participants = await _context.Set<IncidentParticipants>().FromSqlRaw("exec Pro_Incident_GetIncidentByRef_Participant @CompanyID, @IncidentID, @IncidentActionID",
                    pCompanyID2, pIncidentID2, pIncidentActionId2).ToListAsync();
            }

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<IncidentAssets>> IncidentAsset(int CompanyId, int IncidentId, string Source = "WEB")
    {
        DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
        var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
        var pSource = new SqlParameter("@Source", Source);
        var assetlist = await _context.Set<IncidentAssets>().FromSqlRaw("exec Pro_IncidentAsset @CompanyID,@IncidentID, @Source", pCompanyID, pIncidentID, pSource).ToListAsync();

        return assetlist;

    }
    public async Task<IncidentDetails> GetCompanySOS(int CompanyID)
    {
        
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var sosIncident =await _context.Set<GetIncidentByIDResponse>().FromSqlRaw("exec Pro_Incident_GetCompanySOS @CompanyID", pCompanyID).FirstOrDefaultAsync();

            if (sosIncident != null)
            {
                try
                {
                    var pCompanyID2 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.ActionLst = (IEnumerable<ActionLsts>)_context.Set<ActionLsts>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_ActionList @CompanyID", pCompanyID2).AsAsyncEnumerable();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID,0);
                }

                try
                {
                    var pCompanyID3 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.MessageMethods = await _context.Set<CommsMethods>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_MessageMethods @CompanyID", pCompanyID3).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                try
                {
                    var pCompanyID4 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.AckOptions = await _context.Set<AckOption>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_AckOptions @CompanyID", pCompanyID4).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                try
                {
                    var pCompanyID5 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.IncidentAssets = await _context.Set<IncidentAssetResponse>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_IncidentAssets @CompanyID", pCompanyID5).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                try
                {
                    var pCompanyID6 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.ImpactedLocation = await _context.Set<ImpactedLocation?>().FromSqlRaw("Pro_Incident_GetCompanySOS_ImpactedLocation @CompanyID", pCompanyID6).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                try
                {
                    var pCompanyID7 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.NotificationGroups =await  _context.Set<IIncNotificationLst>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_NotificationGroups @CompanyID", pCompanyID7).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                try
                {
                    var pCompanyID8 = new SqlParameter("@CompanyID", CompanyID);
                    sosIncident.IncKeyCon = await _context.Set<IncKeyCons>().FromSqlRaw("exec Pro_Incident_GetCompanySOS_IncKeyCon @CompanyID", pCompanyID8).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CompanyNotFoundException(CompanyID, 0);
                }

                return sosIncident;
            }
          
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task UpdateSOSLocation(int IncidentId, int[] ImpactedLocation)
    {
        
        try
        {
            var imploc = await _context.Set<SosimpactedLocation>().Where(L=> L.IncidentId == IncidentId).ToListAsync();
            _context.RemoveRange(imploc);
            await _context.SaveChangesAsync();

            if (ImpactedLocation != null)
            {
                foreach (int LocId in ImpactedLocation)
                {
                   await AddSOSImpactedLoc(IncidentId, LocId);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task UpdateSOSNotificationGroup(int IncidentId, IncidentNotificationObjLst[] NotificationGroup, int CompanyID)
    {
        try
        {
            var impng =  await _context.Set<SosnotificationGroup>().Where(L=> L.IncidentId == IncidentId).ToListAsync();
            _context.RemoveRange(impng);
            await _context.SaveChangesAsync();

            if (NotificationGroup != null)
            {
                foreach (IncidentNotificationObjLst Obj in NotificationGroup)
                {
                   await AddSOSNotificationGroup(IncidentId, Obj.ObjectMappingId, Obj.SourceObjectPrimaryId, CompanyID);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task AddSOSImpactedLoc(int IncidentID, int LocationID)
    {
        try
        {
            SosimpactedLocation SIL = new SosimpactedLocation();
            SIL.IncidentId = IncidentID;
            SIL.ImpactedLocationId = LocationID;
            await _context.AddAsync(SIL);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task AddSOSNotificationGroup(int IncidentID, int ObjectMapID, int SourceID, int CompanyID)
    {
        try
        {
            SosnotificationGroup SNG = new SosnotificationGroup();
            SNG.IncidentId = IncidentID;
            SNG.ObjectMappingId = ObjectMapID;
            SNG.SourceObjectPrimaryId = SourceID;
            SNG.CompanyId = CompanyID;
            await _context.AddAsync(SNG);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<UpdateIncidentStatusReturn> TestWithMe(int IncidentId, int[] ImpactedLocations, int CurrentUserId, int CompanyId, string TimeZoneId)
    {
        try
        {
            var verifyInci = await _context.Set<Incident>().Where(I=> I.IncidentId == IncidentId && I.CompanyId == CompanyId).FirstOrDefaultAsync();

            if (verifyInci != null)
            {
                IncidentActivation tblIncidenActivation = new IncidentActivation()
                {
                    Name = verifyInci.Name.Trim(),
                    IncidentIcon = verifyInci.IncidentIcon,
                    CompanyId = CompanyId,
                    IncidentId = IncidentId,
                    IncidentDescription = verifyInci.Description.Trim(),
                    Severity = verifyInci.Severity,
                    ImpactedLocationId = ImpactedLocations.FirstOrDefault(),
                    InitiatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId),
                    InitiatedBy = CurrentUserId,
                    LaunchedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId),
                    LaunchedBy = CurrentUserId,
                    Status = 2,
                    TrackUser = verifyInci.TrackUser,
                    SilentMessage = verifyInci.SilentMessage,
                    CreatedBy = CurrentUserId,
                    CreatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId),
                    DeactivatedOn = (DateTime)SqlDateTime.Null,
                    ClosedOn = (DateTime)SqlDateTime.Null,
                    UpdatedBy = CurrentUserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId),
                    AssetId = 0,
                    HasTask = verifyInci.HasTask,
                    LaunchMode = 3,
                    CascadePlanId = verifyInci.CascadePlanId
                };
                await _context.AddAsync(tblIncidenActivation);
                await _context.SaveChangesAsync();

                //Process impacted location
                await ProcessImpactedLocation(ImpactedLocations, tblIncidenActivation.IncidentActivationId, CompanyId, "COMBINED");

                IncidentKeyHldLst[] KCList = new IncidentKeyHldLst[1];
                KCList[0] = new IncidentKeyHldLst { UserId = CurrentUserId };

                await CreateActiveKeyContact(tblIncidenActivation.IncidentActivationId, IncidentId, KCList, CurrentUserId, CompanyId, TimeZoneId);

                int mPriority = SharedKernel.Utils.Common.GetPriority(verifyInci.Severity);

                Messaging MSG = new Messaging(_context,_httpContextAccessor)
                {
                    TimeZoneId = TimeZoneId
                };

                bool MultiResponse = false;
                var pIncidentID = new SqlParameter("@IncidentID", IncidentId);
                //Use: EXEC [dbo].[Pro_ActiveIncident_GetMessageResponse] @IncidentID
                var ackoption =await _context.Set<AckOption>().FromSqlRaw("EXEC Pro_ActiveIncident_GetMessageResponse @IncidentID", pIncidentID).ToListAsync();
                if (ackoption.Count > 0)
                {
                    MultiResponse = true;
                }

                int[] MessageMethod = null;
                var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
                MessageMethod = (from MM in _context.Set<Core.Messages.MessageMethod>()
                                 join CC in _context.Set<CompanyComm>() on MM.MethodId equals CC.MethodId
                                 join CM in _context.Set<CommsMethod>() on MM.MethodId equals CM.CommsMethodId
                                 where MM.IncidentId == IncidentId && CC.ServiceStatus == true && CC.Status == 1
                                 && CC.CompanyId == CompanyId
                                 select CC.MethodId).ToArray();
                    //await _context.Set<IncidentMethod>().FromSqlRaw("exec Pro_Incident_GetMessageMethods @CompanyID,@IncidentID", pCompanyId, pIncidentID).ToArrayAsync();

                //Create Message Records in the Message Table
                MSG.CascadePlanID = verifyInci.CascadePlanId;
                MSG.MessageSourceAction = SourceAction.IncidentTest;
                int tblmessageid =await MSG.CreateMessage(CompanyId, tblIncidenActivation.IncidentDescription, "Incident",
                    tblIncidenActivation.IncidentActivationId, mPriority, CurrentUserId, 1, DateTime.Now.GetDateTimeOffset(TimeZoneId),
                    MultiResponse, ackoption, 99, verifyInci.AudioAssetId, 0, (bool)verifyInci.TrackUser, (bool)verifyInci.SilentMessage,
                    MessageMethod);

                MSG.AddUserToNotify(tblmessageid, new int[] { CurrentUserId }.ToList(), tblIncidenActivation.IncidentActivationId);

                if (verifyInci.HasTask)
                {

                   await _activeIncidentTaskService.StartTaskAllocation(tblIncidenActivation.IncidentId, tblIncidenActivation.IncidentActivationId, CurrentUserId, CompanyId);
                }

                //Copy assets to active incident assets
                var pIncidentID2 = new SqlParameter("@IncidentID", IncidentId);
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", tblIncidenActivation.IncidentActivationId);
                int astrslt =await _context.Database.ExecuteSqlRawAsync("Pro_Create_Active_Assets @IncidentID,@ActiveIncidentID", pIncidentID2, pActiveIncidentID);

                var pMessageId = new SqlParameter("@MessageID", tblmessageid);
                var pIncidentActivationId = new SqlParameter("@IncidentActivationID", tblIncidenActivation.IncidentActivationId);
                var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset(TimeZoneId));

                try
                {
                    int RowsCount = await _context.Database.ExecuteSqlRawAsync("Pro_Create_Launch_Incident_Message_List @IncidentActivationID,@MessageID,@CustomerTime",
                       pIncidentActivationId, pMessageId, pCustomerTime);

                    IsFundAvailable =await MSG.CalculateMessageCost(CompanyId, tblmessageid, tblIncidenActivation.IncidentDescription);

                    await Task.Factory.StartNew(() => QueueHelper.MessageDeviceQueue(tblmessageid, "Incident", 1, verifyInci.CascadePlanId));

                    //QueueHelper.MessageDevicePublish(tblmessageid, 1);
                    QueueConsumer.CreateCascadingJobs(verifyInci.CascadePlanId, tblmessageid, tblIncidenActivation.IncidentActivationId, CompanyId, TimeZoneId);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //write the update status MessageList

                var inciList = await _context.Set<IncidentActivation>()
                                .Where(Incidentval=> Incidentval.CompanyId == CompanyId && Incidentval.IncidentActivationId == tblIncidenActivation.IncidentActivationId
                                ).ToListAsync();

                return  (from Incidentval in inciList
                        select new UpdateIncidentStatusReturn
                        {
                            IncidentActivationId = Incidentval.IncidentActivationId,
                        }).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
           
            throw ex;
        }
    }

    public async Task ProcessImpactedLocation(int[] LocationID, int IncidentActivationID, int CompanyID, string Action)
    {
        try
        {
            var locs = await _context.Set<Location>().Where(L=> LocationID.Contains(L.LocationId)).ToListAsync();
            List<AffectedLocation> AF = new List<AffectedLocation>();
            foreach (var Loc in locs)
            {
                if (IsSOS)
                {
                    Loc.Lat = Latitude;
                    Loc.Long = Longtude;
                }
                AF.Add(new AffectedLocation
                {
                    Address = Loc.PostCode,
                    Lat = Loc.Lat,
                    Lng = Loc.Long,
                    LocationID = 0,
                    LocationName = Loc.LocationName,
                    LocationType = "IMPACTED",
                    ImpactedLocationID = Loc.LocationId
                });
            }
          await  ProcessAffectedLocation(AF, IncidentActivationID, CompanyID, "IMPACTED", Action);
        }
        catch (Exception)
        {

        }
    }

    private async Task ProcessAffectedLocation(List<AffectedLocation> AffectedLocations, int IncidentActivationId,
        int CompanyID, string Type = "AFFECTED", string Action = "INITIATE")
    {

        if (Action == "LAUNCH")
        {
            var ext_loc =await _context.Set<IncidentLocation>().Include(il=>il.IncidentLocationLibrary)
                          .Where(IL=> IL.IncidentActivationId == IncidentActivationId && IL.IncidentLocationLibrary.LocationType == Type &&
                           !AffectedLocations.Select(s => s.LocationID).Contains(IL.LocationId)).ToListAsync();
            _context.RemoveRange(ext_loc);
            await _context.SaveChangesAsync();
        }

        if (AffectedLocations != null)
        {
            foreach (AffectedLocation loc in AffectedLocations)
            {
                CreateIncidentLocation(loc, IncidentActivationId, CompanyID);
            }
        }
    }

    public async Task CreateIncidentLocation(AffectedLocation loc, int ActiveIncidentID, int CompanyID)
    {
        try
        {
           
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pLocationID = new SqlParameter("@LocationID", loc.LocationID);
                var pImpactedLocationID = new SqlParameter("@ImpactedLocationID", loc.ImpactedLocationID);
                var pLocationName = new SqlParameter("@LocationName", loc.LocationName);
                var pLat = new SqlParameter("@Lat", loc.Lat);
                var pLng = new SqlParameter("@Lng", loc.Lng);
                var pAddress = new SqlParameter("@Address", loc.Address);
                var pLocationType = new SqlParameter("@LocationType", loc.LocationType);

               await  _context.Database.ExecuteSqlRawAsync("Pro_Create_Incident_Location @ActiveIncidentID, @CompanyID, @LocationID, @ImpactedLocationID, @LocationName, @Lat, @Lng, @Address, @LocationType",
                    pActiveIncidentID, pCompanyID, pLocationID, pImpactedLocationID, pLocationName, pLat, pLng, pAddress, pLocationType);
           
        }
        catch (Exception ex)
        {
            
            throw ex;
        }

        
    }

    private async Task CreateIncidentNotificationList(int IncidentActivationId, int tblmessageid, IncidentNotificationObjLst[] LaunchIncidentNotificationObjLst, int? UserProfile, int CurrentUserId, int CompanyId, string TimeZoneId)
    {
        try
        {
            var OldNotifyList = await _context.Set<IncidentNotificationList>().Where(ONL=> ONL.CompanyId == CompanyId && ONL.IncidentActivationId == IncidentActivationId).ToListAsync();

            Messaging MSG = new Messaging(_context,_httpContextAccessor);

            List<int> LINOList = new List<int>();
            List<IncidentNotificationObjLst> LstIncNotiLst = new List<IncidentNotificationObjLst>(LaunchIncidentNotificationObjLst);
            foreach (var INotiLst in LstIncNotiLst)
            {
                if (INotiLst.ObjectMappingId > 0)
                {
                    var ISExist = OldNotifyList.FirstOrDefault(s => s.CompanyId == CompanyId
                        && s.IncidentActivationId == IncidentActivationId && s.ObjectMappingId == INotiLst.ObjectMappingId
                        && s.SourceObjectPrimaryId == INotiLst.SourceObjectPrimaryId && s.Status == 1);
                    if (ISExist == null)
                    {
                       await MSG.CreateIncidentNotificationList(tblmessageid, IncidentActivationId, INotiLst.ObjectMappingId, INotiLst.SourceObjectPrimaryId, CurrentUserId, CompanyId, TimeZoneId);
                    }
                    else
                    {
                        LINOList.Add(ISExist.IncidentNotificationListId);
                    }
                }
            }
            foreach (var Ditem in OldNotifyList)
            {
                bool ISDEL = LINOList.Any(s => s == Ditem.IncidentNotificationListId);
                if (!ISDEL)
                {
                    _context.Remove(Ditem);
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async Task CreateActiveKeyContact(int IncidentActivationId, int IncidentId, IncidentKeyHldLst[] LaunchIncidentKeyHldLst, int CurrentUserId, int CompanyId, string TimeZoneId)
    {
        try
        {
            var RActIncConDel = await _context.Set<ActiveIncidentKeyContact>()
                                 .Where(ActIncCon=> ActIncCon.IncidentActivationId == IncidentActivationId).ToListAsync();

            List<int> AIKList = new List<int>();
            List<IncidentKeyHldLst> LstKeyHld = new List<IncidentKeyHldLst>(LaunchIncidentKeyHldLst);
            foreach (var IKeyHld in LstKeyHld)
            {
                if (IKeyHld.UserId != null)
                {
                    var IsExist = RActIncConDel.FirstOrDefault(s => s.IncidentActivationId == IncidentActivationId && s.IncidentId == IncidentId && s.UserId == IKeyHld.UserId.Value);
                    if (IsExist == null)
                    {
                        ActiveIncidentKeyContact tblIncKeyContact = new ActiveIncidentKeyContact()
                        {
                            IncidentActivationId = IncidentActivationId,
                            IncidentId = IncidentId,
                            UserId = IKeyHld.UserId.Value,
                            CreatedBy = CurrentUserId,
                            CreatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId),
                            UpdatedBy = CurrentUserId,
                            UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId)
                        };
                        await _context.AddAsync(tblIncKeyContact);
                    }
                    else
                    {
                        AIKList.Add(IsExist.ActiveIncidentKeyContactId);
                    }
                }
            }
            foreach (var Ditem in RActIncConDel)
            {
                bool ISDEL = AIKList.Any(s => s == Ditem.ActiveIncidentKeyContactId);
                if (!ISDEL)
                {
                    _context.Remove(Ditem);
                }
            }
           await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<CmdMessage>> GetCMDMessage(int ActiveIncidentID, int UserID)
    {
        try
        {
                var pActiveIncidentID = new SqlParameter("@IncidentActivationID", ActiveIncidentID);
                var pUserID = new SqlParameter("@UserID", UserID);
                var result =await _context.Set<CmdMessage>().FromSqlRaw("exec Pro_Incident_CommandCentre_Messages @IncidentActivationID, @UserID",
                    pActiveIncidentID, pUserID).AsQueryable().ToListAsync();

                return result;          
        }
        catch (Exception ex)
        {
           throw ex;
        }
    }
    public async Task<List<MapLocationReturn>> GetIncidentMapLocations(int ActiveIncidentID, string Filter)
    {
        try
        {
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
            var pFilter = new SqlParameter("@Filter", Filter);

            var tckusr = await _context.Set<MapLocationReturn>().FromSqlRaw("exec Pro_ActiveIncident_GetIncidentTracking @ActiveIncidentID, @Filter",
                pActiveIncidentID, pFilter).ToListAsync();

            return tckusr;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> SaveIncidentParticipants(int IncidentId, int ActionID, IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify)
    {
        try
        {
           await IncidentParticipantGroup(IncidentId, ActionID, IncidentParticipants);
           await IncidentParticipantUser(IncidentId, ActionID, UsersToNotify);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> SaveIncidentJob(int CompanyId, int IncidentId, int IncidentActivationId, string Description, int Severity, int[] ImpactedLocationId,
      string TimeZoneId, int CurrentUserId, IncidentKeyHldLst[] IncidentKeyHldLst, IncidentNotificationObjLst[] InitiateIncidentNotificationObjLst,
      bool MultiResponse, List<AckOption> AckOptions, int AssetId = 0, bool TrackUser = false, bool SilentMessage = false, int[] MessageMethod = null,
      List<AffectedLocation> AffectedLocations = null, int[] UsersToNotify = null, List<string> SocialHandle = null)
    {
        DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
        var inci = await _context.Set<IncidentActivation>().Where(I=> I.IncidentActivationId == IncidentActivationId && I.CompanyId == CompanyId).FirstOrDefaultAsync();

        if (inci != null)
        {
            inci.IncidentDescription = Description.Trim();
            inci.Severity = Severity;
            inci.ImpactedLocationId = ImpactedLocationId[0];
            inci.TrackUser = TrackUser;
            inci.SilentMessage = SilentMessage;
            inci.UpdatedBy = CurrentUserId;
            inci.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
            inci.AssetId = AssetId;
            inci.SocialHandle = string.Join(",", SocialHandle);
            _context.Update(inci);
            await _context.SaveChangesAsync();

            CleanUPJob(IncidentActivationId);

            if (MessageMethod != null)
            {
                if (MessageMethod.Length > 0)
                {
                    bool pushadded = false;
                    int pushmethodid = 1;
                    if (TrackUser)
                    {
                        pushmethodid = await _context.Set<CommsMethod>().Where(w => w.MethodCode == "PUSH").Select(s => s.CommsMethodId).FirstOrDefaultAsync();
                    }

                    Messaging MSG = new Messaging(_context,_httpContextAccessor);
                    foreach (int Method in MessageMethod)
                    {
                       await  MSG.CreateMessageMethod(0, Method, inci.IncidentActivationId);
                        if (pushmethodid == Method)
                            pushadded = true;
                    }
                    if (TrackUser && !pushadded)
                    {
                       await MSG.CreateMessageMethod(0, pushmethodid, inci.IncidentActivationId);
                    }
                }
            }

            if (UsersToNotify != null)
            {
                Messaging MSG = new Messaging(_context, _httpContextAccessor);
                MSG.AddUserToNotify(0, UsersToNotify.ToList(), inci.IncidentActivationId);
            }

            if (MultiResponse)
            {
                Messaging MSG = new Messaging(_context, _httpContextAccessor);
                await MSG.SaveActiveMessageResponse(0, AckOptions, inci.IncidentActivationId);
            }

            //Process impacted location
           await  ProcessImpactedLocation(ImpactedLocationId, inci.IncidentActivationId, CompanyId, "INITIATE");

            //Manage affected locations
           await  ProcessAffectedLocation(AffectedLocations, inci.IncidentActivationId, CompanyId, "AFFECTED", "INITIATE");

            List<IncidentKeyHldLst> LstKeyHld = new List<IncidentKeyHldLst>(IncidentKeyHldLst);
            foreach (var IKeyHld in LstKeyHld)
            {
                if (IKeyHld.UserId != null)
                {
                    ActiveIncidentKeyContact tblIncKeyContact = new ActiveIncidentKeyContact()
                    {
                        IncidentActivationId = inci.IncidentActivationId,
                        IncidentId = IncidentId,
                        UserId = IKeyHld.UserId.Value,
                        CreatedBy = CurrentUserId,
                        CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                        UpdatedBy = CurrentUserId,
                        UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
                    };
                    await _context.AddAsync(tblIncKeyContact);
                    await _context.SaveChangesAsync();
                }
            }

            List<IncidentNotificationObjLst> LstIncNotiLst = new List<IncidentNotificationObjLst>(InitiateIncidentNotificationObjLst);
            foreach (var INotiLst in LstIncNotiLst)
            {
                if (INotiLst.ObjectMappingId > 0)
                {
                    IncidentNotificationList tblIncidentNotiLst = new IncidentNotificationList()
                    {
                        CompanyId = CompanyId,
                        IncidentActivationId = inci.IncidentActivationId,
                        ObjectMappingId = INotiLst.ObjectMappingId,
                        SourceObjectPrimaryId = INotiLst.SourceObjectPrimaryId,
                        MessageId = 0,
                        Status = 1,
                        CreatedBy = CurrentUserId,
                        CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                        UpdatedBy = CurrentUserId,
                        UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
                    };
                    await _context.AddAsync(tblIncidentNotiLst);
                    await _context.SaveChangesAsync();
                }
            }

            var inciList =  await _context.Set<IncidentActivation>()
                            .Where(Incidentval=> Incidentval.CompanyId == CompanyId && Incidentval.IncidentActivationId == inci.IncidentActivationId
                           ).ToListAsync();
            try
            {
                return true;
            }
            catch (Exception ex)
            {
              
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task CleanUPJob(int ActiveIncidentID)
    {
        try
        {
          
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                await _context.Database.ExecuteSqlRawAsync("Pro_Job_Data_Cleanup @ActiveIncidentID", pActiveIncidentID);
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<IncidentSegLinks>> SegregationLinks(int TargetID,string MemberShipType, string LinkType,int OutUserCompanyId)
    {
        try
        {

            var pGroupID = new SqlParameter("@IncidnetID", TargetID);
            var pLinkType = new SqlParameter("@LinkType", LinkType);
            var pMemberShipType = new SqlParameter("@MemberShipType", MemberShipType);
            var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);

            var result = await _context.Set<IncidentSegLinks>().FromSqlRaw("exec Pro_Get_Incident_Seg_Links @IncidnetID, @LinkType, @MemberShipType, @CompanyID",
                pGroupID, pLinkType, pMemberShipType, pCompanyID).ToListAsync();
            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> UpdateSegregationLink(int SourceID, int TargetID,  string LinkType, int CurrentUserId, int CompanyId)
    {
        try
        {
            if (LinkType == "DEPARTMENT")
            {
                var item =  await _context.Set<SegIncidentDepartmentLink>().Where(I=> I.IncidentId == SourceID && I.DepartmentId == TargetID).FirstOrDefaultAsync();
                if (item == null)
                {
                   await  CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                }
                else if ( item != null)
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            else if (LinkType == "GROUP")
            {
                var item = await _context.Set<SegGroupIncidentLink>().Where(I=> I.IncidentId == SourceID && I.GroupId == TargetID).FirstOrDefaultAsync();
                if ( item == null)
                {
                   await  CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                }
                else if (item != null)
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            else if (LinkType == "LOCATION")
            {
                var item = await _context.Set<SegIncidentLocationLink>().Where(I=> I.IncidentId == SourceID && I.LocationId == TargetID).FirstOrDefaultAsync();
                if ( item == null)
                {
                    await CreateSegregtionLink(SourceID, TargetID, LinkType, CompanyId);
                }
                else if ( item != null)
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private async Task CreateSegregtionLink(int sourceID, int targetID, string LinkType, int companyId)
    {
        try
        {
            if (LinkType == "DEPARTMENT")
            {
                var targtetitem = await _context.Set<Department>().Where(w => w.DepartmentId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                if (targtetitem != null)
                {
                    SegIncidentDepartmentLink SGL = new SegIncidentDepartmentLink()
                    {
                        CompanyId = companyId,
                        DepartmentId = targetID,
                        IncidentId = sourceID
                    };
                    await _context.AddAsync(SGL);
                    await _context.SaveChangesAsync();
                }
            }
            else if (LinkType == "GROUP")
            {
                var targtetitem = await _context.Set<Group>().Where(w => w.GroupId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                if (targtetitem != null)
                {
                    SegGroupIncidentLink SGL = new SegGroupIncidentLink()
                    {
                        CompanyId = companyId,
                        IncidentId = sourceID,
                        GroupId = targetID
                    };
                    await _context.AddAsync(SGL);
                    await _context.SaveChangesAsync();
                }
            }
            else if (LinkType == "LOCATION")
            {
                var targtetitem = await _context.Set<Location>().Where(w => w.LocationId == targetID && w.CompanyId == companyId).FirstOrDefaultAsync();
                if (targtetitem != null)
                {
                    SegIncidentLocationLink SGL = new SegIncidentLocationLink()
                    {
                        CompanyId = companyId,
                        LocationId = targetID,
                        IncidentId = sourceID
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
    public async Task<List<MessageGroupObject>> GetIncidentRecipientEntity(int ActiveIncidentID, string EntityType, int TargetUserId, int CompanyId)
    {
        try
        {
          
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                var pEntityType = new SqlParameter("@EntityType", EntityType);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                var pUserID = new SqlParameter("@UserID", TargetUserId);

                var result =await _context.Set<MessageGroupObject>().FromSqlRaw("exec Pro_Get_Incident_Recipient_Entity @ActiveIncidentID, @EntityType, @CompanyID, @UserID",
                    pActiveIncidentID, pEntityType, pCompanyID, pUserID).ToListAsync();

                return result;
           
        }
        catch (Exception ex)
        {
          return new List<MessageGroupObject>();
        }
    }


    public async Task<DataTablePaging> GetIncidentEntityRecipient(int start, int length, Search search,int draw,string orderBy,string dir ,int activeIncidentID, string entityType, int entityID, int companyId, int currentUserId, string companyKey)
    {
        try
        {

            var RecordStart = start == 0 ? 0 : start;
            var RecordLength = length == 0 ? int.MaxValue : length;
            var SearchString = (search != null) ? search.Value : "";
            string OrderBy = orderBy != null ? orderBy : "UserId";
            string OrderDir = dir != string.Empty ? dir : "desc";
            OrderBy = string.IsNullOrEmpty(OrderBy) ? "UserId" : OrderBy;

            var returnData =await GetIncidentEntityRecipientData(activeIncidentID,entityType, entityID,companyId, currentUserId,
                RecordStart, RecordLength, SearchString, OrderBy, OrderDir, companyKey);

            int totalRecord = 0;

            if (returnData != null)
                totalRecord = returnData.Count();

            List<EntityRcpntResponse> ttodata = await GetIncidentEntityRecipientData(activeIncidentID, entityType, entityID, companyId,
                currentUserId, 0, int.MaxValue, "", "UserId", OrderDir, companyKey);

            if (ttodata != null)
                totalRecord = ttodata.Count;


            DataTablePaging rtn = new DataTablePaging();
            rtn.Draw = draw;
            rtn.RecordsTotal = totalRecord;
            rtn.RecordsFiltered = returnData.Count;
            rtn.Data = returnData;

            if (rtn != null)
            {
                return rtn;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<EntityRcpntResponse>> GetIncidentEntityRecipientData(int ActiveIncidentID, string EntityType, int EntityID, int CompanyId, int UserId,
           int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir, string CompanyKey)
    {
        try
        {

           
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                var pEntityType = new SqlParameter("@EntityType", EntityType);
                var pEntityID = new SqlParameter("@EntityID", EntityID);
                var pCompanyId = new SqlParameter("@CompanyId", CompanyId);
                var pUserId = new SqlParameter("@UserId", UserId);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = new List<EntityRcpntResponse>();
                var propertyInfo = typeof(EntityRcpntResponse).GetProperty(OrderBy);

                if (OrderDir == "desc")
                {
                    result =await _context.Set<EntityRcpntResponse>().FromSqlRaw("exec Pro_Get_Incident_Group_Recipient @ActiveIncidentID, @EntityType,@EntityID, @CompanyID, @UserId, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pActiveIncidentID, pEntityType, pEntityID, pCompanyId, pUserId, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToListAsync();
                }
                else
                {
                    result =await _context.Set<EntityRcpntResponse>().FromSqlRaw("exec Pro_Get_Incident_Group_Recipient @ActiveIncidentID, @EntityType,@EntityID, @CompanyID, @UserId, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pActiveIncidentID, pEntityType, pEntityID, pCompanyId, pUserId, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .OrderBy(o => propertyInfo.GetValue(o, null)).ToListAsync();
                }

                return result;
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}