using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IIncidentRepository
{
    Task<bool> CheckDuplicate(int companyId, string incidentName, int incidentId);

    Task<Incident?> GetIncident(int companyId, int incidentId);

    Task<int> AddIncident(Incident incident);

    Task AddIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken);

    Task UpdateIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken);

    Task<IncidentActivation?> GetIncidentActivation(int companyId, int incidentActivationId);

    Task AddIncidentKeyContacts(ICollection<IncidentKeyContact> contacts);

    Task ProcessKeyHolders(int companyId, int incidentId, int currentUserId, int[] keyHolders);

    Task SaveIncidentMessageResponse(ICollection<AckOption> ackOptions, int incidentId);

    Task AddIncidentGroup(int incidentId, int[] groups, int companyId);

    Task CreateIncidentSegLinks(int incidentId, int userId, int companyId);

    NewIncident CloneIncident(int incidentId, bool keepKeyContact, bool keepIncidentMessage, bool keepTasks,
        bool keepIncidentAsset, bool keepTaskAssets,
        bool keepTaskCheckList, bool keepIncidentParticipants, int status, int currentUserId, int companyId,
        string timeZoneId);

    Task<ICollection<DataIncidentType>> CopyIncidentTypes(int userId, int companyId);

    Task CopyIncidentToCompany(int companyId, int userId, string timeZoneId = "GMT Standard Time");

    Task<string> GetStatusName(int status);
    List<IncidentList> GetCompanyIncident(int CompanyId, int UserID);
    List<IncidentTypeReturn> CompanyIncidentType(int CompanyId);
    List<AffectedLocation> GetAffectedLocation(int CompanyId, string? LocationType);
    List<AffectedLocation> GetIncidentLocation(int CompanyId, int IncidentActivationId);
    List<CommsMethods> GetIncidentComms(int ItemID, string Type);
    IncidentDetails GetIncidentById(int CompanyId, int UserID, int IncidentId, string UserStatus);
    Task<int> AddCompanyIncidents(
        int CompanyId, string IncidentIcon, string Name, string Description, int PlanAssetID,
            int IncidentTypeId, int Severity, int NumberOfKeyHolders, int CurrentUserId, string TimeZoneId,
            AddIncidentKeyHldLst[] AddIncidentKeyHldLst, int AudioAssetId, int Status = 1, bool TrackUser = false,
            bool SilentMessage = false, List<AckOption> AckOptions = null, bool IsSOS = false, int[] MessageMethod = null, int CascadePlanID = 0,
            int[] Groups = null, int[] Keyholders = null);
    Task AttachKeyContactsToIncident(int IncidentID, int UserID, int CompanyID, AddIncidentKeyHldLst[] KCList, string TimeZoneID);
    Task ProcessKeyholders(int CompanyId, int IncidentId, int CurrentUserId, int[] Keyholders);
    Task<int> ActivateSampleIncident(int UserID, int CompanyID, string TimeZoneID);
    Task<int> CreateSOSIncident(int UserID, int CompanyID, string TimeZoneID);
    Task CheckSOSIncident(int CompanyID, int UserID, string TimeZoneID);
    Task<List<Attachment>> _get_attachments(int ObjectID, string AttachmentType);
    Task<List<IncidentTask>> GetNotes(int ObjectID, string NoteType, bool GetAttachments, string AttachmentType, int CompanyId);
    Task<List<Attachment>> GetAttachments(int ObjectID, string AttachmentType);
    Task<List<IncidentMessagesRtn>> GetIndidentTimeline(int IncidentActivationID, int CompanyID, int UserID);
    Task<int> IncidentNote(int ObjectID, string NoteType, string Notes, int CompanyID, int UserID);
    Task<bool> AddIncidentNote(int ActiveIncidentID, string Note, List<Attachment> Attachments, int UserID, int CompanyID, string TimeZoneId);
    Task<bool> InsertAttachment(int ObjectID, string Title, string OrigFileName, string FileName, string MimeType, string AttachmentType, decimal? FileSize);
    Task<CallToAction> GetCallToAction(int ActiveIncidentID, int UserID, int CompanyID, string TimeZoneId);
    Task<int> CheckUserSOS(int ActiveIncidentID, int UserID);
    Task<bool> UpdateSOS(int SOSAlertID, int UserID, string SOSClosureNotes, bool CloseSOS, bool CloseAllSOS,
            bool MultiNotes, int[] CaseNoteIDs, bool CloseSOSIncident, int ActiveIncidentID, int CurrentUserId, int CompanyId, string TimeZoneId);
    Task<UpdateIncidentStatusReturn> UpdateIncidentStatus(int CompanyId, int IncidentActivationId, string Type, string TimeZoneId, int CurrentUserId,
            string UserRole, string Reason, int NumberOfKeyHolder, string CompletionNotes = "", int[] MessageMethod = null, int CascadePlanID = 0, bool isSos = false);
    Task<List<IncidentSOSRequest>> GetIncidentSOSRequest(int IncidentActivationId);
    Task<IEnumerable<UpdateIncident>> GetActiveIncidentBasic(int CompanyId, int IncidentActivationId);
    Task<int> UpdateIncidentType(string Name, int IncidentTypeId, int UserId, int CompanyId);
    Task DeleteEmptyTaskHeader(int IncidentID);
    Task<int> UpdateCompanyIncidents(int CompanyId, int IncidentId, string IncidentIcon, string Name, string Description,
      int PlanAssetID, int IncidentTypeId, int Severity, int Status, int NumberOfKeyHolders, int CurrentUserId, string TimeZoneId,
      UpdIncidentKeyHldLst[] UpdIncidentKeyHldLst, int AudioAssetId, bool TrackUser, bool SilentMessage, List<AckOption> AckOptions,
      int[] MessageMethod, int CascadePlanID, int[] Groups, int[] Keyholders);
    Task<List<IncidentAssets>> AddIncidentAssets(int CompanyId, int IncidentId, string LinkedAssetId, int CurrentUserId, string TimeZoneId);
    Task<TourIncident> GetIncidentByName(string IncidentName, int CompanyId, int UserId, string TimeZoneId);
    Task IncidentParticipantGroup(int IncidentId, int ActionID, IncidentNotificationObjLst[] IncidentParticipants);
    Task IncidentParticipantUser(int IncidentId, int ActionID, int[] UsersToNotify);
    Task CreateIncidentParticipant(int IncidentID, int GroupID, int ObjectMapID, int IncidentActionID, int UserID, string ParticipantType);
    Task<List<ActionLsts>> AddIncidentActions(int CompanyId, int IncidentId, string Title, string ActionDescription,
          IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify, int Status, int CurrentUserId, string TimeZoneId);
    Task<bool> DeleteIncidentAssets(int CompanyId, int IncidentId, int AssetObjMapId, int IncidentAssetId);
    Task<bool> DeleteCompanyIncidentActions(int CompanyId, int IncidentId, int IncidentActionId, int CurrentUserId, string TimeZoneId);
    Task<bool> DeleteCompanyIncidents(int CompanyId, int IncidentId, int CurrentUserId, string TimeZoneId);
    Task<List<ActionLsts>> UpdateCompanyIncidentActions(int CompanyId, int IncidentId, int IncidentActionId, string Title, string ActionDescription,
           IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify, int Status, int CurrentUserId, string TimeZoneId);
    Task<UpdateIncidentStatus> ActiveIncidentDetailsById(int CompanyId, int IncidentActivationId, int CurrentUserID, bool isapp = false);
    Task<List<IncidentLibraryDetails>> IncidentLibrary(int CompanyId);
    Task<List<ActionLsts>> IncidentMessage(int CompanyId, int IncidentId);
    Task<ActionLsts> GetIncidentAction(int CompanyId, int IncidentId, int IncidentActionId);
    Task<List<IncidentAssets>> IncidentAsset(int CompanyId, int IncidentId, string Source = "WEB");
    Task<IncidentDetails> GetCompanySOS(int CompanyID);
    Task AddSOSNotificationGroup(int IncidentID, int ObjectMapID, int SourceID, int CompanyID);
    Task AddSOSImpactedLoc(int IncidentID, int LocationID);
    Task UpdateSOSNotificationGroup(int IncidentId, IncidentNotificationObjLst[] NotificationGroup, int CompanyID);
    Task UpdateSOSLocation(int IncidentId, int[] ImpactedLocation);
    Task<List<CmdMessage>> GetCMDMessage(int ActiveIncidentID, int UserID);
    Task CreateIncidentLocation(AffectedLocation loc, int ActiveIncidentID, int CompanyID);
    Task ProcessImpactedLocation(int[] LocationID, int IncidentActivationID, int CompanyID, string Action);
    Task<UpdateIncidentStatusReturn> TestWithMe(int IncidentId, int[] ImpactedLocations, int CurrentUserId, int CompanyId, string TimeZoneId);
    Task CleanUPJob(int ActiveIncidentID);
    Task<bool> SaveIncidentJob(int CompanyId, int IncidentId, int IncidentActivationId, string Description, int Severity, int[] ImpactedLocationId,
      string TimeZoneId, int CurrentUserId, IncidentKeyHldLst[] IncidentKeyHldLst, IncidentNotificationObjLst[] InitiateIncidentNotificationObjLst,
      bool MultiResponse, List<AckOption> AckOptions, int AssetId = 0, bool TrackUser = false, bool SilentMessage = false, int[] MessageMethod = null,
      List<AffectedLocation> AffectedLocations = null, int[] UsersToNotify = null, List<string> SocialHandle = null);
    Task<bool> SaveIncidentParticipants(int IncidentId, int ActionID, IncidentNotificationObjLst[] IncidentParticipants, int[] UsersToNotify);
    Task<List<MapLocationReturn>> GetIncidentMapLocations(int ActiveIncidentID, string Filter);
    Task<bool> UpdateSegregationLink(int SourceID, int TargetID, string LinkType, int CurrentUserId, int CompanyId);
    Task<List<IncidentSegLinks>> SegregationLinks(int TargetID, string MemberShipType, string LinkType, int OutUserCompanyId);
    Task<DataTablePaging> GetIncidentEntityRecipient(int start, int length, string search, int draw, string orderBy, string dir, int activeIncidentID, string entityType, int entityID, int companyId, int currentUserId, string companyKey);
    Task<List<EntityRcpntResponse>> GetIncidentEntityRecipientData(int ActiveIncidentID, string EntityType, int EntityID, int CompanyId, int UserId,
           int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir, string CompanyKey);
    Task<List<MessageGroupObject>> GetIncidentRecipientEntity(int ActiveIncidentID, string EntityType, int TargetUserId, int CompanyId);
}