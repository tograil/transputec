namespace CrisesControl.Core.Incidents;

public class AffectedLocation
{
    public int LocationID { get; set; }
    public string LocationName { get; set; }
    public string Address { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }
    public string LocationType { get; set; }
    public int? ImpactedLocationID { get; set; }
    public int? IncidentActivationID { get; set; }
}