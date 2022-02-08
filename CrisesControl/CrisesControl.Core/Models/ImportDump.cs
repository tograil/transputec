using System;

namespace CrisesControl.Core.Models
{
    public partial class ImportDump
    {
        public int ImportDumpId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Isd { get; set; }
        public string? Phone { get; set; }
        public string? Llisd { get; set; }
        public string? Landline { get; set; }
        public string? UserRole { get; set; }
        public string? Action { get; set; }
        public string? Status { get; set; }
        public string? Group { get; set; }
        public string? GroupStatus { get; set; }
        public string? GroupAction { get; set; }
        public string? Location { get; set; }
        public string? LocationAddress { get; set; }
        public string? LocationStatus { get; set; }
        public string? LocationAction { get; set; }
        public int? SecurityGroupId { get; set; }
        public string? Security { get; set; }
        public string? PingMethods { get; set; }
        public string? IncidentMethods { get; set; }
        public string? EmailCheck { get; set; }
        public string? LocationCheck { get; set; }
        public string? GroupCheck { get; set; }
        public string? SecurityCheck { get; set; }
        public string? ImportAction { get; set; }
        public string? ActionType { get; set; }
        public string? ActionCheck { get; set; }
        public string? ValidationMessage { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? LocationId { get; set; }
        public int? GroupId { get; set; }
        public string? LocLat { get; set; }
        public string? LocLng { get; set; }
        public string? EncryptedEmail { get; set; }
        public int? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? DepartmentAction { get; set; }
        public string? DepartmentCheck { get; set; }
        public string? DepartmentStatus { get; set; }
    }
}
