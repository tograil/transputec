namespace CrisesControl.Core.Incidents
{
    public class IncidentList
    {
        public int CompanyId { get; set; }
        public int IncidentId { get; set; }
        public string Name { get; set; }
        public string IncidentIcon { get; set; }
        public int Severity { get; set; }
        public int NumberOfKeyHolders { get; set; }
        public int Status { get; set; }
        public string IncidentTypeName { get; set; }
        public bool HasTask { get; set; }
        public int SegregationWarning { get; set; }
    }
}
