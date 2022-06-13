using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class DeliverySummary
    {
        public string? CountryPhoneCode { get; set; }
        public string? CountryName { get; set; }
        public int? Delivered { get; set; }
        public int? Queued { get; set; }
        public int? UnDelivered { get; set; }
        public int? Failed { get; set; }
        public int? Accepted { get; set; }
        public int? Received { get; set; }
        public int? Sent { get; set; }
        public int? PFailed { get; set; }
        public int? PhoneQueued { get; set; }
        public int? Busy { get; set; }
        public int? NoAnswer { get; set; }
        public int? Completed { get; set; }
        public int? TotalText { get; set; }
        public int? TotalPhone { get; set; }
        public int? EmailSent { get; set; }
        public int? EmailFailed { get; set; }
        public int? PushFailed { get; set; }
        public int? PushSent { get; set; }
        public int? TotalEmail { get; set; }
        public int? TotalPush { get; set; }
        public DateTimeOffset TextStartTime { get; set; }
        public DateTimeOffset TextEndTime { get; set; }
        public DateTimeOffset PhoneStartTime { get; set; }
        public DateTimeOffset PhoneEndTime { get; set; }
        public DateTimeOffset EmailStartTime { get; set; }
        public DateTimeOffset EmailEndTime { get; set; }
        public DateTimeOffset PushStartTime { get; set; }
        public DateTimeOffset PushEndTime { get; set; }
        public bool? BeforeCutOff { get; set; }
        public bool? EmailUsed { get; set; }
        public bool? PushUsed { get; set; }
        public bool? TextUsed { get; set; }
        public bool? PhoneUsed { get; set; }
        public int? NoAppUser { get; set; }
    }
}
