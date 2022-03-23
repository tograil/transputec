using System.Collections.Generic;

namespace CrisesControl.Core.Incidents;

public class ActionLsts
{
    public int IncidentActionId { get; set; }
    public int IncidentId { get; set; }
    public string Title { get; set; }
    public string ActionDescription { get; set; }
    public int MultiResponse { get; set; }
    public int Status { get; set; }
    public int CompanyId { get; set; }
    public IEnumerable<IncKeyActCons> IncKeyActCon { get; set; }
    public List<IncidentParticipants> Participants { get; set; }
    public bool HasParticipants { get; set; }
}