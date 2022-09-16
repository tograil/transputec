﻿using System;
using CrisesControl.Core.Departments;

namespace CrisesControl.Core.Models
{
    public partial class AdhocMessageNotificationList
    {
        public int AdhocMessageNotificationListId { get; set; }
        public int MessageId { get; set; }
        public int CompanyId { get; set; }
        public int ObjectMappingId { get; set; }
        public int SourceObjectPrimaryId { get; set; }
        public int? UserProfileId { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public Department Department { get; set; }
    }
}
