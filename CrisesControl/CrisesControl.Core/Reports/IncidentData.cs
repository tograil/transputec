using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Reports;

public class IncidentData
{
    public int IncidentId { get; set; }
    public bool TrackUser { get; set; }
    public int IncidentActivationId { get; set; }
    public string IncidentIcon { get; set; }
    public int IncidentSeverity { get; set; }
    public string IncidentPlan { get; set; }
    public string IncidentName { get; set; }
    public string IncidentLocation { get; set; }
    public string ActivatedByFirstName { get; set; }
    public string ActivatedByLastName { get; set; }
    public DateTimeOffset ActivatedOn { get; set; }
    public string LaunchedByFirstName { get; set; }
    public string LaunchedByLastName { get; set; }
    public DateTimeOffset LaunchedOn { get; set; }
    public string DeActivatedByFirstName { get; set; }
    public string DeActivatedByLastName { get; set; }
    public DateTimeOffset DeactivatedOn { get; set; }
    public string ClosedByFirstName { get; set; }
    public string ClosedByLastName { get; set; }
    public DateTimeOffset ClosedOn { get; set; }
    public int CurrentStatus { get; set; }
    public int Counferences { get; set; }
    public bool HasNotes { get; set; }
    public bool HasTask { get; set; }
    public bool IsKeyContact { get; set; }
    public List<KeyContact>? KeyContacts { get; set; }
    public List<IncidentAssets>? IncidentAssets { get; set; }
}