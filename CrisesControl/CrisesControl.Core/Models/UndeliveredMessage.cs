using System;

namespace CrisesControl.Core.Models
{
    public partial class UndeliveredMessage
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public int MessageId { get; set; }
        public long MessageDeviceId { get; set; }
        public string MethodName { get; set; } = null!;
        public int Attempt { get; set; }
        public int ScheduleFlag { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
