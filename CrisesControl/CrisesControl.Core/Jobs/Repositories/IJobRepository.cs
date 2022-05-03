﻿using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs.Repositories;

    public interface IJobRepository
    {
        Task<IEnumerable<JobList>> GetAllJobs(int CompanyID, int UserID);
        Task<IEnumerable<JobList>> GetJob(int CompanyID, int JobId);
    }

