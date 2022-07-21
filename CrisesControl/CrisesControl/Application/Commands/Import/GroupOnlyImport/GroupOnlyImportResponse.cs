namespace CrisesControl.Api.Application.Commands.Import.GroupOnlyImport
{
    public class GroupOnlyImportResponse
    {
        public int UserImportTotalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string GroupName { get; set; }
        public string GroupStatus { get; set; }
        public string GroupCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
    }
}
