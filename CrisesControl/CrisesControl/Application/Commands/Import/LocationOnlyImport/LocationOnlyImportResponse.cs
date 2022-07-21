namespace CrisesControl.Api.Application.Commands.Import.LocationOnlyImport
{
    public class LocationOnlyImportResponse
    {
        public int UserImportTotalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string LocationStatus { get; set; }
        public string LocationCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
    }
}
