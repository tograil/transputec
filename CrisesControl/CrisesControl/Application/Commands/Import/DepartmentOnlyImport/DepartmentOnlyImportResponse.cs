namespace CrisesControl.Api.Application.Commands.Import.DepartmentOnlyImport
{
    public class DepartmentOnlyImportResponse
    {
        public int UserImportTotalId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentStatus { get; set; }
        public string DepartmentCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
    }
}
