using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Sop;
using CrisesControl.Core.Sop.Respositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Jobs;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SopRepository: ISopRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<SopRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SopRepository(IHttpContextAccessor httpContextAccessor, ILogger<SopRepository> logger, CrisesControlContext context)
        {
           this._context = context;
           this._logger = logger;
           this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<LibSopSection>> GetSOPSectionLibrary()
        {
            try
            {
                var sections = await  _context.Set<LibSopSection>().Where(CS=> CS.Status == 1).ToListAsync();
                return sections;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task<GetSopResponse> GetSOP(int CompanyId,int SOPHeaderID = 0)
        {
            try
            {
                GetSopResponse getSop = new GetSopResponse();

                if (SOPHeaderID > 0)
                {
                    var pSOPHeaderID = new SqlParameter("@SOPHeaderID", SOPHeaderID);
                    var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
                    var sop =  _context.Set<SOP>().FromSqlRaw("exec Pro_Get_SOP @SOPHeaderID,@CompanyID").ToList()
                               .Select(U=> 
                               {
                                   U.SOPOwnerName = new UserFullName { Firstname = U.SOPOwnerName.Firstname, Lastname = U.SOPOwnerName.Lastname };
                                   return U;
                                   
                               }).FirstOrDefault();
                    getSop.SOP = sop;
                }
                else
                {
                    var sop = await _context.Set<SOPList>().FromSqlRaw("exec Pro_Get_SOP @SOPHeaderID,@CompanyID").ToListAsync();
                    sop.Select(U =>
                    {
                        U.Author = new UserFullName { Firstname = U.Author.Firstname, Lastname = U.Author.Lastname };
                        return U;

                    }).ToList();
                    getSop.SOPLists = sop;
                }
                return getSop;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<int> SaveSOPHeader(Sopheader SOPHeader)
        {
            await _context.AddAsync(SOPHeader);
            await _context.SaveChangesAsync();
            return SOPHeader.SopheaderId;

        }

        public async Task<int> UPdateSOPHeader(Sopheader SOPHeader)
        {
             _context.Update(SOPHeader);
            await _context.SaveChangesAsync();
            return SOPHeader.SopheaderId;
        }
        public async Task LinkSOPWithIncident(int SOPHeaderID, int IncidentID,int CurrentUserID,string TimeZoneId)
        {

            try
            {
                var sop_incident = await  _context.Set<IncidentSop>().Where(SI=> SI.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                if (sop_incident != null)
                {
                    if (sop_incident.IncidentId != IncidentID)
                    {
                        sop_incident.UpdatedBy = CurrentUserID;
                        sop_incident.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        sop_incident.IncidentId = IncidentID;
                        _context.Update(sop_incident);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    IncidentSop ISOP = new IncidentSop();
                    ISOP.IncidentId = IncidentID;
                    ISOP.SopheaderId = SOPHeaderID;
                    ISOP.CreatedBy = CurrentUserID;
                    ISOP.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    ISOP.UpdatedBy = CurrentUserID;
                    ISOP.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(ISOP);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Sopheader> GetSopheaderById( int SOPHeaderID)
        {
           var sopHeader= await _context.Set<Sopheader>().Where(SH=> SH.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
            return sopHeader;
        }

        public async Task<Profit> GetCompanyProfile(int CompanyID)
        {
            var profile = await  _context.Set<CompanyPaymentProfile>().Where(w => w.CompanyId == CompanyID).Select(s => new { s.SoptokenValue, s.Vatrate }).FirstOrDefaultAsync();
            Profit profit = new Profit();
            if (profile!=null)
            {
                profit.SOPTokenValue = profile.SoptokenValue;
                profit.VATRate = profile.Vatrate;

            }
            return profit;
        }
        public async Task<int> GetTransactionTypeID(string TransactionCode)
        {
            int TextTransacTypeId = await  _context.Set<TransactionType>().Where(w => w.TransactionCode == TransactionCode).Select(s => s.TransactionTypeId).FirstOrDefaultAsync();
            return TextTransacTypeId;
        }

        public async Task<int> UpdateTransactionDetails(int TransactionHeaderId, int CompanyId, int TransactionTypeId, decimal TransactionRate, decimal MinimumPrice,
           int Quantity, decimal Cost, decimal LineValue, decimal LineVAT, decimal Total, int MessageId, DateTimeOffset TransactionDate, int currntUserId = 0,
           string TransactionReference = "", string TimeZoneId = "GMT Standard Time", int TransactionStatus = 1, int TransactionDetailsId = 0, string TrType = "DR")
        {

            int TDId = 0;
            try
            {
                if (Total > 0)
                {
                    if (TransactionDetailsId == 0)
                    {

                        TransactionDetail NewTransactionDetails = new TransactionDetail()
                        {
                            TransactionHeaderId = TransactionHeaderId,
                            CompanyId = CompanyId,
                            TransactionReference = TransactionReference,
                            TransactionTypeId = TransactionTypeId,
                            TransactionRate = TransactionRate,
                            MinimumPrice = MinimumPrice,
                            TransactionDate = TransactionDate,
                            Quantity = Quantity,
                            Cost = Cost,
                            LineValue = LineValue,
                            LineVat = LineVAT,
                            Total = Total,
                            TransactionStatus = TransactionStatus,
                            MessageId = MessageId,
                            CreatedBy = currntUserId,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = currntUserId,
                            UpdateOn = DateTime.Now,
                            Drcr = TrType,
                            IsPaid = false
                        };
                        await _context.AddAsync(NewTransactionDetails);
                        await _context.SaveChangesAsync();
                        TDId = NewTransactionDetails.TransactionDetailsId;

                    }
                    else
                    {
                        var newTransactionDetails = await _context.Set<TransactionDetail>().Where(TD=> TD.TransactionDetailsId == TransactionDetailsId).FirstOrDefaultAsync();
                        if (newTransactionDetails != null)
                        {
                            newTransactionDetails.TransactionHeaderId = TransactionHeaderId;
                            newTransactionDetails.TransactionReference = TransactionReference;
                            newTransactionDetails.TransactionTypeId = TransactionTypeId;
                            newTransactionDetails.TransactionRate = TransactionRate;
                            newTransactionDetails.MinimumPrice = MinimumPrice;
                            newTransactionDetails.Quantity = Quantity;
                            newTransactionDetails.LineValue = LineValue;
                            newTransactionDetails.Cost = Cost;
                            newTransactionDetails.LineVat = LineVAT;
                            newTransactionDetails.Total = Total;
                            newTransactionDetails.TransactionDate = TransactionDate;
                            newTransactionDetails.TransactionStatus = TransactionStatus;
                            newTransactionDetails.UpdatedBy = currntUserId;
                            newTransactionDetails.UpdateOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                            newTransactionDetails.Drcr = TrType;
                            _context.Update(newTransactionDetails);
                            await _context.SaveChangesAsync();
                            TDId = newTransactionDetails.TransactionDetailsId;
                        }
                    }
                }
                return TDId;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            
        }
        public async Task CreateSOPReviewReminder(int IncidentID, int SOPHeaderID, int CompanyID, DateTimeOffset NextReviewDate, string ReviewFrequency, int ReminderCount)
        {
            try
            {
                DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
                DBC.DeleteScheduledJob("SOP_REVIEW_" + SOPHeaderID, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "SOP_REVIEW_" + SOPHeaderID;
                string taskTrigger = "SOP_REVIEW_" + SOPHeaderID;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(SOPReviewJob));
                jobDetail.JobDataMap["IncidentID"] = IncidentID;
                jobDetail.JobDataMap["SOPHeaderID"] = SOPHeaderID;

                int Counter = 0;
                DateTimeOffset DateCheck = DBC.GetNextReviewDate(NextReviewDate, CompanyID, ReminderCount, out Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                var sop_head = await _context.Set<Sopheader>().Where(SH=> SH.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                sop_head.ReminderCount = Counter;
                _context.Update(sop_head);
                await _context.SaveChangesAsync();

                //if(DateCheck.Date >= DateTime.Now.Date && Counter <= 3) {
                if (DateTimeOffset.Compare(DateCheck, DBC.GetDateTimeOffset(DateTime.Now)) >= 0 && Counter <= 3)
                {
                    //string TimeZoneVal = DBC.GetTimeZoneByCompany(CompanyID);
                    //DateCheck = DBC.GetServerTime(TimeZoneVal, DateCheck);

                    if (DateCheck < DateTime.Now)
                        DateCheck = DateTime.Now.AddMinutes(5);

                    //DBC.UpdateLog("0", jobName + ", Starting at: " + DateCheck.ToUniversalTime().ToString(), "CreateTasksReviewReminder", "CreateTasksReviewReminder", 0);

                    ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                                                              .WithIdentity(taskTrigger, "REVIEW_REMINDER")
                                                              .StartAt(DateCheck.ToUniversalTime())
                                                              .ForJob(jobDetail)
                                                              .Build();
                   await _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    DateTimeOffset NewReviewDate = DBC.GetNextReviewDate(NextReviewDate, ReviewFrequency);

                    if (sop_head != null)
                    {
                        sop_head.ReviewDate = NewReviewDate;
                        sop_head.ReminderCount = 0;
                        _context.Update(sop_head);
                        await _context.SaveChangesAsync();
                        await CreateSOPReviewReminder(IncidentID, SOPHeaderID, CompanyID, NewReviewDate, ReviewFrequency, 0);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
