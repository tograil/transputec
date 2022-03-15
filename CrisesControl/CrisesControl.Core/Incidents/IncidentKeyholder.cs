namespace CrisesControl.Core.Incidents;

public class IncidentKeyholder
{
    public long IncidentKeyholderID { get; set; }
    public int? IncidentID { get; set; }
    public int? ActiveIncidentID { get; set; }
    public int? UserID { get; set; }
    public int? CompanyID { get; set; }
}