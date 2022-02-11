using System;

namespace CrisesControl.Core.Models
{
    public partial class Sosalert
    {
        public int SosalertId { get; set; }
        public int? UserId { get; set; }
        public int? ActiveIncidentId { get; set; }
        public int? MessageId { get; set; }
        public int? MessageListId { get; set; }
        public string? AlertType { get; set; }
        public int? ResponseId { get; set; }
        public string? ResponseLabel { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTimeOffset? ResponseTime { get; set; }
        public DateTimeOffset? ResponseTimeGmt { get; set; }
        public int? CallbackOption { get; set; }
        public bool? Completed { get; set; }
        public int? CompletedBy { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
        public int? UserContactedBy { get; set; }
        public DateTimeOffset? UserContactedDate { get; set; }
    }
}
