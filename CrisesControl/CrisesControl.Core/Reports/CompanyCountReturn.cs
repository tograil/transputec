﻿using CrisesControl.Core.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class CompanyCountReturn:CommonDTO
    {
        public int CompanyId { get; set; }
        public long TotalPushCount { get; set; }
        public long TotalEmailCount { get; set; }
        public long TotalTextCount { get; set; }
        public long TotalPhoneCount { get; set; }
        public List<CompanyUserCountReturn> CompanyUserCountReturn { get; set; }
    }
}
