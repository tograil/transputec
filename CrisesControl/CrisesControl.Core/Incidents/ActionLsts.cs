using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents;

public class ActionLsts
{
    public int IncidentActionId { get; set; }
    public int IncidentId { get; set; }
    public string Title { get; set; }
    public string ActionDescription { get; set; }
    [NotMapped]
    public int MultiResponse { get; set; }
    public int Status { get; set; }
    public int CompanyId { get; set; }
    [NotMapped]
    public IEnumerable<IncKeyActCons> IncKeyActCon { get; set; }
    [NotMapped]
    public List<IncidentParticipants> Participants { get; set; }
    public bool HasParticipants { get; set; }
}