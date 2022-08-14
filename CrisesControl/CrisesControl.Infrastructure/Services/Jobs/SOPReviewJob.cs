using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services.Jobs
{
    public class SOPReviewJob : IJob
    {
        private readonly CrisesControlContext db;
        private readonly DBCommon DBC ;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SOPReviewJob(IHttpContextAccessor httpContextAccessor)
        {
            this.DBC = new DBCommon(db, _httpContextAccessor);
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
                        SendEmail SE = new SendEmail(db, DBC);
                        SE.SendReviewAlert(IncidentID, incident.SH.SopheaderId, incident.I.CompanyId, "SOP");

                        incident.SH.ReminderCount = Counter;
                        db.SaveChanges();
                       
                        await DBC.CreateSOPReviewReminder(IncidentID, incident.SH.SopheaderId, incident.I.CompanyId, incident.SH.ReviewDate, incident.SH.ReviewFrequency, Counter);

                    }
                    else
                    {
                        DBC.DeleteScheduledJob("SOP_REVIEW_" + SOPHeaderID, "REVIEW_REMINDER");
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
