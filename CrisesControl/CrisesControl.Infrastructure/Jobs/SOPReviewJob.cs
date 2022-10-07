using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Sop.Respositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Repositories;
using CrisesControl.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Jobs
{
    public class SOPReviewJob : IJob
    {
        private readonly CrisesControlContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISopRepository _sopRepository;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;
        public SOPReviewJob(CrisesControlContext context, IHttpContextAccessor httpContextAccessor, ISopRepository sopRepository, IDBCommonRepository _DBC, ISenderEmailService SDE)
        {
            db = context;
            _httpContextAccessor = httpContextAccessor;
            _sopRepository = sopRepository;
            DBC = _DBC;
            _SDE = SDE;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            int IncidentID = context.JobDetail.JobDataMap.GetInt("IncidentID");
            int SOPHeaderID = context.JobDetail.JobDataMap.GetInt("SOPHeaderID");
            int Counter = context.JobDetail.JobDataMap.GetInt("Counter");

            try
            {
                var incident = (from I in db.Set<Incident>()
                                join TH in db.Set<IncidentSop>() on I.IncidentId equals TH.IncidentId
                                join SH in db.Set<Sopheader>() on TH.SopheaderId equals SH.SopheaderId
                                where I.IncidentId == IncidentID && SH.SopheaderId == SOPHeaderID
                                select new { I, SH }).FirstOrDefault();
                if (incident != null)
                {
                    if (incident.I.Status == 1 && incident.SH.Status == 1)
                    {
                       
                       await _SDE.SendReviewAlert(IncidentID, incident.SH.SopheaderId, (int)incident.I.CompanyId, "SOP");

                        incident.SH.ReminderCount = Counter;
                        db.SaveChanges();

                        
                        await _sopRepository.CreateSOPReviewReminder(IncidentID, incident.SH.SopheaderId, (int)incident.I.CompanyId, incident.SH.ReviewDate, incident.SH.ReviewFrequency, Counter);

                    }
                    else
                    {
                       await DBC.DeleteScheduledJob("SOP_REVIEW_" + SOPHeaderID, "REVIEW_REMINDER");
                    }
                }
                await Task.WhenAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
