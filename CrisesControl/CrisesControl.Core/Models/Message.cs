using System;

namespace CrisesControl.Core.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public string? MessageText { get; set; }
        public string? MessageType { get; set; }
        public int IncidentActivationId { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset CreatedTimeZone { get; set; }
        public int AssetId { get; set; }
        public int ActiveIncidentTaskId { get; set; }
        public int Source { get; set; }
        public bool MultiResponse { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public int? AttachmentCount { get; set; }
        public bool Text { get; set; }
        public bool Phone { get; set; }
        public bool Push { get; set; }
        public bool Email { get; set; }
        public int ParentId { get; set; }
        public int HasReply { get; set; }
        public int? MessageActionType { get; set; }
        public int CascadePlanId { get; set; }
        public string? MessageSourceAction { get; set; }
    }
}
