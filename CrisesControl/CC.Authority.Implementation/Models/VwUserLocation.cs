using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class VwUserLocation
    {
        public int RowId { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
    }
}
