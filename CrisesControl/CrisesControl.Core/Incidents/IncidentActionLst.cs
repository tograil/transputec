namespace CrisesControl.Core.Incidents;

public class IncidentActionLst
{
    public int IncidentActionId { get; set; }
    public string ActionDescription { get; set; }
    public InitiateIncidentActionKeyHldLst[] InitiateIncidentActionKeyHldLst { get; set; }
}