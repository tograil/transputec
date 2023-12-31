﻿using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.ExTriggers.Repositories
{
    public interface IExTriggerRepository
    {
        Task<IEnumerable<ExTriggerList>> GetAllExTrigger();
        Task<IEnumerable<ExTriggerList>> GetExTrigger(int ExTriggerID, int CompanyID);
        Task<IEnumerable<ExTriggerList>> GetImpTrigger();
    }
}
