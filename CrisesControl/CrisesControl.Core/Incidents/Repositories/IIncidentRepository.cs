﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

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
}