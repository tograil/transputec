﻿using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class ObjectRelation
    {
        public int ObjectRelationId { get; set; }
        public int TargetObjectPrimaryId { get; set; }
        public int SourceObjectPrimaryId { get; set; }
        public int ObjectMappingId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public bool ReceiveOnly { get; set; }
    }
}
