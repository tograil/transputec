using CrisesControl.Core.Companies;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Sop;
using CrisesControl.Core.Sop.Respositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Jobs;
using CrisesControl.Infrastructure.Services;
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
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;
        private readonly string TimeZone = "GMT Standard Time";
        public SopRepository(IHttpContextAccessor httpContextAccessor, ILogger<SopRepository> logger, CrisesControlContext context, IDBCommonRepository _DBC, ISenderEmailService SDE)
        {
           this._context = context;
           this._logger = logger;
           this._httpContextAccessor = httpContextAccessor;
           this.DBC = _DBC;
            this._SDE = SDE;
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
        public async Task<bool> AttachSOPToIncident(int SOPHeaderID, string SOPFileName, int CurrentUserID,int CompanyID, string TimeZoneId="GMT Standard Time")
        {
            try
            {
                var sop = await _context.Set<IncidentSop>().Where(SH=>SH.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                if (sop != null)
                {
                    var incident = await _context.Set<Incident>().Where(I=> I.IncidentId == sop.IncidentId).FirstOrDefaultAsync();
                    if (incident != null)
                    {
                        //incident.IncidentPlanDoc = SOPFileName;
                        incident.IsSopdoc = true;
                        incident.PlanAssetId = sop.AssetId;
                        incident.SopdocId = SOPHeaderID;
                        incident.UpdatedBy = CurrentUserID;
                        incident.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        _context.Update(incident);
                        await _context.SaveChangesAsync();

                        var asset = await _context.Set<Assets>().Where(A=> A.AssetId == sop.AssetId).FirstOrDefaultAsync();
                        if (asset != null)
                        {
                            asset.AssetTypeId = 5;
                            _context.Update(asset);
                            await _context.SaveChangesAsync();
                        }

                       
                        await _SDE.NotifyKeyContactForSOPAttach(incident.IncidentId, CompanyID);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
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
        // TODO : Linq to Store Procedure
        public async Task<List<ContentSectionData>> GetSOPSections(int SOPHeaderID, int CompanyID, int ContentSectionID = 0)
        {
            try
            {
                if (ContentSectionID <= 0)
                {
                    var SectionOwners = (from SDG in _context.Set<SopdetailGroup>()
                                         join SD in _context.Set<Sopdetail>() on SDG.SopdetailId equals SD.SopdetailId
                                         join U in _context.Set<User>() on SDG.SopgroupId equals U.UserId
                                         where SD.SopheaderId == SOPHeaderID && U.Status == 1
                                         select new { SDG.SopgroupId, U.FirstName, U.LastName, SD.ContentSectionId }).ToList();

                    var sections = await (from CS in _context.Set<ContentSection>()
                                          join SD in _context.Set<Sopdetail>() on CS.ContentSectionId equals SD.ContentSectionId
                                          join C in _context.Set<Content>() on SD.ContentId equals C.ContentId
                                          join SH in _context.Set<Sopheader>() on CS.SopheaderId equals SH.SopheaderId
                                          where CS.SopheaderId == SOPHeaderID && SH.CompanyId == CompanyID && CS.SectionName != "BRIEF_DESCRIPTION"
                                          select new ContentSectionData
                                          {
                                              UpdatedOn = SH.UpdatedOn,
                                              SOPHeaderID = SH.SopheaderId,
                                              ContentSectionID = CS.ContentSectionId,
                                              ContentID = C.ContentId,
                                              SectionType = CS.SectionType,
                                              SectionDescription = C.ContentText,
                                              SectionName = CS.SectionName,
                                              SectionOrder = CS.SectionOrder,
                                              SOPContentTags = (from CT in _context.Set<ContentTag>()
                                                                join T in _context.Set<Tag>() on CT.TagId equals T.TagId
                                                                where CT.ContentId == C.ContentId
                                                                select new SOPContentTag { TagID = CT.TagId }).ToList()
                                          }).OrderBy(o => o.SectionOrder).ToListAsync();
                    sections.Select(c =>
                    {
                        c.SectionGroups = SectionOwners.Where(w => w.ContentSectionId == c.ContentSectionID).Select(s => new SectionGroup
                        {
                            SOPGroupID = s.SopgroupId,
                            OwnerName = new UserFullName { Firstname = s.FirstName, Lastname = s.LastName }
                        }
                            ).ToList();
                        return c;
                    });

                    return sections;
                }
                else
                {
                    var SectionOwners = (from SDG in _context.Set<SopdetailGroup>()
                                         join SD in _context.Set<Sopdetail>() on SDG.SopdetailId equals SD.SopdetailId
                                         join U in _context.Set<User>() on SDG.SopgroupId equals U.UserId
                                         where SD.SopheaderId == SOPHeaderID && SD.ContentSectionId == ContentSectionID && U.Status == 1
                                         select new SectionGroup
                                         {
                                             SOPGroupID = SDG.SopgroupId,
                                             OwnerName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName }
                                         }).ToList();

                    var section = await (from CS in _context.Set<ContentSection>()
                                         join SD in _context.Set<Sopdetail>() on CS.ContentSectionId equals SD.ContentSectionId
                                         join C in _context.Set<Content>() on SD.ContentId equals C.ContentId
                                         join SH in _context.Set<Sopheader>() on CS.SopheaderId equals SH.SopheaderId
                                         where CS.SopheaderId == SOPHeaderID && SH.CompanyId == CompanyID && CS.ContentSectionId == ContentSectionID
                                         select new ContentSectionData
                                         {
                                             UpdatedOn = SH.UpdatedOn,
                                             SOPHeaderID = SH.SopheaderId,
                                             ContentSectionID = CS.ContentSectionId,
                                             ContentID = C.ContentId,
                                             SectionType = CS.SectionType,
                                             SectionDescription = C.ContentText,
                                             SectionName = CS.SectionName,
                                             SectionOrder = CS.SectionOrder,
                                             SOPContentTags = (from CT in _context.Set<ContentTag>()
                                                               join T in _context.Set<Tag>() on CT.TagId equals T.TagId
                                                               where CT.ContentId == C.ContentId
                                                               select new SOPContentTag { TagID = CT.TagId }).ToList()
                                         }).ToListAsync();
                    section.Select(c =>
                    {
                        c.SectionGroups = SectionOwners;

                        return c;
                    });
                    return section;
                }                
               

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
                
                await DBC.DeleteScheduledJob("SOP_REVIEW_" + SOPHeaderID, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "SOP_REVIEW_" + SOPHeaderID;
                string taskTrigger = "SOP_REVIEW_" + SOPHeaderID;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(SOPReviewJob));
                jobDetail.JobDataMap["IncidentID"] = IncidentID;
                jobDetail.JobDataMap["SOPHeaderID"] = SOPHeaderID;

                int Counter = 0;
                DateTimeOffset DateCheck =await DBC.GetNextReviewDate(NextReviewDate, CompanyID, ReminderCount,  Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                var sop_head = await _context.Set<Sopheader>().Where(SH=> SH.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                sop_head.ReminderCount = Counter;
                _context.Update(sop_head);
                await _context.SaveChangesAsync();

                //if(DateCheck.Date >= DateTime.Now.Date && Counter <= 3) {
                if (DateTimeOffset.Compare(DateCheck,await DBC.GetDateTimeOffset(DateTime.Now, TimeZone)) >= 0 && Counter <= 3)
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
                    DateTimeOffset NewReviewDate =await DBC.GetNextReviewDate(NextReviewDate, ReviewFrequency);

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
        public async Task<int> AU_ContentSection(int ContentSectionID, int SOPHeaderID, string SectionName, int Status, string SectionType, int SectionOrder, int CurrentUserID, string TimeZoneId="GMT STandard Time")
        {
            try
            {
                int ContentId = 0;
                if (ContentSectionID > 0)
                {
                    var content = await _context.Set<ContentSection>().Where(CS=> CS.ContentSectionId == ContentSectionID && CS.SopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                    if (content != null)
                    {
                        content.SectionName = SectionName;
                        content.SectionType = SectionType;
                        content.UpdatedBy = CurrentUserID;
                        content.Status = Status;
                        //content.SectionOrder = SectionOrder;
                        content.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        _context.Update(content);
                        await _context.SaveChangesAsync();
                        ContentId= content.ContentSectionId;
                    }
                }
                else
                {
                    ContentSection content = new ContentSection();
                    content.SectionName = SectionName;
                    content.SectionType = SectionType;
                    content.Status = Status;
                    content.SopheaderId = SOPHeaderID;
                    content.SectionOrder = SectionOrder;
                    content.CreatedBy = CurrentUserID;
                    content.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    content.UpdatedBy = CurrentUserID;
                    content.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await  _context.AddAsync(content);
                    await _context.SaveChangesAsync();
                    ContentId= content.ContentSectionId;
                }
                return ContentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task<int> AU_SOPHeader(int SOPHeaderID, int IncidentID, string SOPVersion, int SOPOwner, string BriefDescription, DateTimeOffset NextReviewDate, string ReviewFrequency,
                                int ContentID, int ContentSectionID,int CurrentUserID,int CompanyId, int Status = 1, string TimeZoneId= "GMT Standard Time")
        {
            try
            {
                UsageCalculation _usagecalc = new UsageCalculation(DBC,_SDE);
                
               
                int Rt_SopHeaderId = 0;
                int ReminderCount = 0;
                if (SOPHeaderID > 0)
                {
                    var sop_head = await GetSopheaderById(SOPHeaderID);
                    if (sop_head != null)
                    {
                        sop_head.Sopowner = SOPOwner;
                        sop_head.ReviewDate = NextReviewDate;
                        sop_head.ReviewFrequency =ReviewFrequency;
                        sop_head.ReminderCount = ReminderCount;
                        sop_head.Sopversion = SOPVersion;
                        sop_head.UpdatedBy = CurrentUserID;
                        sop_head.Status = 1;
                        sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        var updatedId = await UPdateSOPHeader(sop_head);

                        await LinkSOPWithIncident(updatedId, IncidentID, CurrentUserID, TimeZoneId);
                        Rt_SopHeaderId = updatedId;
                        ReminderCount = sop_head.ReminderCount;
                    }
                }
                else
                {
                    Sopheader sop_head = new Sopheader();
                    sop_head.ReviewDate = NextReviewDate;
                    sop_head.Sopowner = SOPOwner;
                    sop_head.ReviewFrequency = ReviewFrequency;
                    sop_head.CompanyId = CompanyId;
                    sop_head.Sopversion = SOPVersion;
                    sop_head.CreatedBy = CurrentUserID;
                    sop_head.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    sop_head.Status = 1;
                    sop_head.ReminderCount = ReminderCount;
                    sop_head.UpdatedBy = CurrentUserID;
                    sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    var NewSopheaderID = await SaveSOPHeader(sop_head);

                    await LinkSOPWithIncident(NewSopheaderID, IncidentID, CurrentUserID, TimeZoneId);

                    Rt_SopHeaderId = NewSopheaderID;
                    //using (var dbContextTransaction = db.Database.BeginTransaction())
                    //{

                    int TextTransacTypeId = await GetTransactionTypeID("ISOPUSAGE");

                    decimal VATRate = 20M;
                    var profile = await GetCompanyProfile(CompanyId);
                    VATRate = (decimal)profile.VATRate;

                    decimal VatValue = (profile.SOPTokenValue * VATRate) / 100;
                    decimal iSOPNetValue = profile.SOPTokenValue - VatValue;

                    await UpdateTransactionDetails(0, CompanyId, TextTransacTypeId, iSOPNetValue, iSOPNetValue, 1, iSOPNetValue, iSOPNetValue, VatValue, profile.SOPTokenValue,
                             Rt_SopHeaderId, DateTime.Now, CurrentUserID, "SOPDOC" + Rt_SopHeaderId.ToString(),TimeZoneId);

                    await _usagecalc.update_company_balance(CompanyId, profile.SOPTokenValue);

                }
                if (Status != 1)
                {
                   await DBC.DeleteScheduledJob("SOP_REVIEW_" + Rt_SopHeaderId, "REVIEW_REMINDER");
                }
                else
                {
                    await CreateSOPReviewReminder(IncidentID, Rt_SopHeaderId, CompanyId, NextReviewDate, ReviewFrequency, ReminderCount);
                }

                //Creating the contents for breif description
                int rtContentSectionID = await AU_ContentSection(ContentSectionID, Rt_SopHeaderId, "BRIEF_DESCRIPTION", 1, "INFO", 0,CurrentUserID,TimeZoneId);
                int rtContentID = await AU_Content(ContentID, BriefDescription, "INFO", 1,CurrentUserID,TimeZoneId);
               int SopdetailId=await AU_SOPDetail(Rt_SopHeaderId, rtContentID, rtContentSectionID);

                return Rt_SopHeaderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> AU_Content(int ContentID, string ContentText, string ContentType, int Status, int CurrentUserID, string TimeZoneId = "GMT Standard Time")
         {
            try
            {
                int PrimaryContentID = 0;
                string old_checksum = "";
                string Checksum = ContentText.Trim().PWDencrypt();

                if (ContentID > 0)
                {
                    var old_content =  await _context.Set<Content>().Where(C=> C.ContentId == ContentID).FirstOrDefaultAsync();
                    if (old_content != null)
                    {
                        old_checksum = old_content.Checksum;
                        if (old_checksum != Checksum)
                        {
                            PrimaryContentID = old_content.PrimaryContentId == 0 ? old_content.ContentId : old_content.ContentId;
                            old_content.Status = 2;
                           await CreateContentVersion(PrimaryContentID, ContentID, CurrentUserID,TimeZoneId);
                        }
                    }
                }

                if (old_checksum.Trim() != Checksum.Trim())
                {
                    Content content = new Content();
                    content.ContentText = ContentText;
                    content.ContentType = ContentType;
                    content.Status = Status;
                    content.CreatedBy = CurrentUserID;
                    content.Checksum = Checksum;
                    content.PrimaryContentId = PrimaryContentID;
                    content.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    content.UpdatedBy = CurrentUserID;
                    content.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                    await _context.AddAsync(content);
                    await _context.SaveChangesAsync();
                    return content.ContentId;
                }
                return ContentID;

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task CreateContentVersion(int PrimaryContentID, int LastContentID, int CurrentUserID, string TimeZoneId= "GMT Standard Time")
        {
            try
            {
                ContentVersion content = new ContentVersion();
                content.PrimaryContentId = PrimaryContentID;
                content.LastContentId = LastContentID;
                content.CreatedBy = CurrentUserID;
                content.CreatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                await _context.AddAsync(content);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AU_SOPDetail(int SOPHeaderID, int ContentID, int ContentSectionID)
        {
            int sop_detail_id = 0;
            try
            {
                var old_content = await _context.Set<Sopdetail>()
                                   .Where(SD=> SD.SopheaderId == SOPHeaderID && SD.ContentSectionId == ContentSectionID
                                   ).FirstOrDefaultAsync();
                if (old_content != null)
                {
                    old_content.ContentId = ContentID;
                     _context.Update(old_content);
                    await _context.SaveChangesAsync();
                    sop_detail_id = old_content.SopdetailId;
                    //return old_content.SOPDetailID;
                }
                else
                {
                    Sopdetail sop_detail = new Sopdetail();
                    sop_detail.SopheaderId = SOPHeaderID;
                    sop_detail.ContentId = ContentID;
                    sop_detail.ContentSectionId = ContentSectionID;
                    await _context.AddAsync(sop_detail);
                    await _context.SaveChangesAsync();
                    sop_detail_id = sop_detail.SopheaderId;
                }
                return sop_detail_id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<List<ContentTags>> GetContentTags()
        {
            try
            {
                var tag_list =await _context.Set<Tag>().Include(TG=>TG.TagCategory)
                                .Select(T=> new ContentTags {TagId= T.TagId, TagCategoryId=T.TagCategoryId,TagName= T.TagName, SearchTerms=T.SearchTerms, TagCategoryName=T.TagCategory.TagCategoryName,TagCategorySearchTerms= T.TagCategory.TagCategorySearchTerms }).ToListAsync();
                return tag_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<int> ReorderSection(List<Section> SectionOrder)
        {
            foreach (Section section in SectionOrder)
            {
                var r = await _context.Set<ContentSection>().Where(w => w.ContentSectionId == section.ContentSectionID).FirstOrDefaultAsync();
                r.SectionOrder = section.SectionOrder;
                _context.Update(r);
                await _context.SaveChangesAsync();
            }
            return 1;
        }
        public async Task AU_ContentTag(int ContentID, List<int> Tags, int CurrentUserID,string TimeZoneId= "GMT Standard Time")
        {
            try
            {
                var existing = await _context.Set<ContentTag>().Where(c => c.ContentId == ContentID).Select(s => new { s.TagId, s.ContentTagId }).ToListAsync();
                var existingtags = existing.Select(s => s.TagId).ToList();
                var todelete = existing.Where(c => !Tags.Contains(c.TagId)).Select(s => s.ContentTagId).ToList();

                var newitems = Tags.Where(tw => !existingtags.Contains(tw)).Select(s => s).ToList();

                if (todelete.Count > 0)
                {
                    _context.RemoveRange(_context.Set<ContentTag>().Where(_ => todelete.Contains(_.ContentTagId)));
                    await _context.SaveChangesAsync();
                }

                foreach (int TagID in newitems)
                {
                    ContentTag tags = new ContentTag();
                    tags.TagId = TagID;
                    tags.ContentId = ContentID;
                    tags.CreatedBy = CurrentUserID;
                    tags.CreatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                    tags.UpdatedBy = CurrentUserID;
                    tags.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(tags);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AU_SOPGroupDetail(int SOPDetailID, List<int> Groups, int CurrentUserID, string TimeZoneId= "GMT Standard Time")
        {
            try
            {
                var existing = await _context.Set<SopdetailGroup>().Where(c => c.SopdetailId == SOPDetailID).Select(s => new { s.SopgroupId, s.SopdetailGroupId }).ToListAsync();
                var existinggroups = existing.Select(s => s.SopgroupId).ToList();
                var todelete = existing.Where(c => !Groups.Any(a => a == c.SopgroupId)).Select(s => s.SopdetailGroupId).ToList();

                var newitems = Groups.Where(c => !existinggroups.Contains(c)).Select(s => s).ToList();

                if (todelete.Count > 0)
                {
                    _context.RemoveRange(_context.Set<SopdetailGroup>().Where(_ => todelete.Contains(_.SopdetailGroupId)));
                    await _context.SaveChangesAsync();
                }

                foreach (int GroupID in newitems)
                {
                    SopdetailGroup group = new SopdetailGroup();
                    group.SopgroupId = GroupID;
                    group.SopdetailGroupId = SOPDetailID;
                    group.CreatedBy = CurrentUserID;
                    group.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    group.UpdatedBy = CurrentUserID;
                    group.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(group);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AU_Section(int SOPHeaderID, int ContentID, int ContentSectionID, string SectionType, string SectionName, string SectionDescription, int SectionStatus, int SectionOrder,
            List<int> Groups, List<int> Tags, int CurrentUserId)
        {
            int rtContentSectionID = 0;
            try
            {
                int rtContentID = 0;
                int rtSOPDetailID = 0;

                rtContentSectionID = await AU_ContentSection(ContentSectionID, SOPHeaderID, SectionName, SectionStatus, SectionType, SectionOrder, CurrentUserId);

                if (rtContentSectionID > 0)
                    rtContentID = await  AU_Content(ContentID, SectionDescription, "INFO", 1, CurrentUserId);

                if (rtContentID > 0 && rtContentSectionID > 0)
                    rtSOPDetailID = await  AU_SOPDetail(SOPHeaderID, rtContentID, rtContentSectionID);

                if (Groups.Count > 0 && rtSOPDetailID > 0)
                   await AU_SOPGroupDetail(rtSOPDetailID, Groups, CurrentUserId);

                if (Tags.Count > 0 && rtContentID > 0)
                   await AU_ContentTag(rtContentID, Tags,CurrentUserId);
                return rtContentSectionID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          

        }

        public async Task DeleteSections(int ContentSectionID)
        {
            try
            {
                var content = await (from C in _context.Set<Content>()
                               join SD in _context.Set<Sopdetail>() on C.ContentId equals SD.ContentId
                               where SD.ContentSectionId == ContentSectionID
                               select C).FirstOrDefaultAsync();
                if (content != null)
                {
                    var content_version = await (from C in _context.Set<Content>()
                                           join SP in _context.Set<Sopdetail>() on C.ContentId equals SP.ContentId
                                           join CS in _context.Set<ContentSection>() on SP.ContentSectionId equals CS.ContentSectionId
                                           where ((C.PrimaryContentId == content.PrimaryContentId && C.PrimaryContentId != 0) || C.ContentId == content.ContentId) &&
                                           CS.SectionName != "BRIEF_DESCRIPTION"
                                           select new { C, CS }).ToListAsync();

                    foreach (var cversion in content_version)
                    {
                        //Remove content tags
                        var tags = _context.Set<ContentTag>().Where(T=> T.ContentId == cversion.C.ContentId).ToListAsync();
                        _context.RemoveRange(tags);

                        //Remove groups association
                        var grp = (from G in _context.Set<SopdetailGroup>()
                                   join SD in _context.Set<Sopdetail>() on G.SopdetailId equals SD.SopdetailId
                                   where SD.ContentId == cversion.C.ContentId
                                   select new { G, SD }).ToList();

                        _context.RemoveRange(grp.Select(s => s.G));
                        _context.RemoveRange(grp.Select(s => s.SD));
                    }
                    _context.RemoveRange(content_version.Select(s => s.C));
                    _context.RemoveRange(content_version.Select(s => s.CS));
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LibraryText>> GetLibraryText(string IncidentName, string IncidentType, string Sector, string SectionTitle)
        {
            try
            {
                var pSectionTitle = new SqlParameter("@SectionTitle", SectionTitle);
                var pIncidentName = new SqlParameter("@IncidentName", IncidentName);
                var pIncidentType = new SqlParameter("@IncidentType", IncidentType);
                var sections = await _context.Set<LibraryText>().FromSqlRaw("exec Pro_Get_Library_Text @SectionTitle,@IncidentType, @IncidentName", pSectionTitle, pIncidentType, pIncidentName).ToListAsync();
                             sections.Select(SH=> new
                                {
                                    Author = new UserFullName { Firstname = SH.Author.Firstname, Lastname = SH.Author.Lastname },
                                    SH.CompanyName,
                                    SH.LibSOPHeaderID,
                                    SH.LibSectionID,
                                    SH.LibContentID,
                                    SH.SectionDescription,
                                    SH.SectionName,
                                    SH.UpdatedOn,
                                    SH.TotalVotes,
                                    SH.NoOfUse,
                                    SH.TotalRating,
                                    SOPContentTags =  _context.Set<ContentTag>().Include(x=>x.Tag)
                                                      .Where(CT=> CT.ContentId == SH.LibContentID)
                                                      .Select(CT=> new { CT.TagId }).ToList()
                                }).ToList();
                return sections;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteSOP(int LibSOPHeaderID, int CurrentUserID, int CompanyID, string TimeZoneId="GMT Standard Time")
        {

            try
            {
                var section = await _context.Set<Sopheader>().Include(i=>i.IncidentSOP)
                               .Where(SH=> SH.SopheaderId == LibSOPHeaderID && SH.CompanyId == CompanyID).FirstOrDefaultAsync();
                if (section != null)
                {

                    section.Status = 3;
                    section.UpdatedBy = CurrentUserID;
                    section.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);

                    var incident = await _context.Set<Incident>().Where(I=> I.IncidentId == section.IncidentSOP.IncidentId).FirstOrDefaultAsync();
                    if (incident != null)
                    {
                        incident.IsSopdoc = false;
                        incident.SopdocId = 0;
                    }
                    _context.Update(incident);
                   await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
              
                return false;
            }
        }
        public async Task<bool> UpdateSOPAsset(int SOPHeaderID, int AssetID, int CurrentUserID, int CompanyID, string TimeZoneId="GMT Standard Time")
        {

            try
            {
                var section = await _context.Set<Sopheader>().Include(i => i.IncidentSOP)
                               .Where(SH => SH.SopheaderId == SOPHeaderID && SH.CompanyId == CompanyID).FirstOrDefaultAsync();
                if (section != null)
                {
                    section.IncidentSOP.AssetId = AssetID;
                    section.IncidentSOP.UpdatedBy = CurrentUserID;
                    section.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
