namespace CrisesControl.Core.Incidents;

public class LaunchIncidentActionLst
{
    public int IncidentActionId { get; set; }
    /// <summary>
    /// Message text
    /// </summary>
    public string ActionDescription { get; set; }

    public LaunchIncidentActionKeyHldLst[] LaunchIncidentActionKeyHldLst { get; set; }
}