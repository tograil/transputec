using System;

namespace CrisesControl.Core.Models
{
    public partial class SecurityObject
    {
        public int SecurityObjectId { get; set; }
        public int ParentId { get; set; }
        public int TypeId { get; set; }
        public string? SecurityKey { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Target { get; set; }
        public int Status { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int RoleId { get; set; }
        public bool RequireKeyHolder { get; set; }
        public decimal MenuOrder { get; set; }
        public int? ModuleAccess { get; set; }
        public bool RequireAdmin { get; set; }
        public bool? ShowOnTrial { get; set; }
        public int ForIncidentManager { get; set; }
    }
}
