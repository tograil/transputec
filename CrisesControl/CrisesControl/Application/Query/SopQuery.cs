using CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Models;
using CrisesControl.Core.Sop.Respositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class SopQuery : ISopQuery
    {
        private readonly ISopRepository _sopRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<SopQuery> _logger;
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public SopQuery(ISopRepository sopRepository, ICurrentUser currentUser, ILogger<SopQuery> logger,CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._currentUser = currentUser;
            this._sopRepository = sopRepository;
            this._logger = logger;
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<SaveSOPHeaderResponse> SaveSOPHeader(SaveSOPHeaderRequest request)
        {
            try
            {
                UsageCalculation _usagecalc = new UsageCalculation();
                DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
                var response = new SaveSOPHeaderResponse();
                int Rt_SopHeaderId = 0;
                int ReminderCount = 0;
                if (request.SOPHeaderID > 0)
                {
                    var sop_head = await _sopRepository.GetSopheaderById(request.SOPHeaderID); 
                    if (sop_head != null)
                    {
                        sop_head.Sopowner = request.SOPOwner;
                        sop_head.ReviewDate = request.ReviewDate;
                        sop_head.ReviewFrequency = request.ReviewFrequency;
                        sop_head.ReminderCount = ReminderCount;
                        sop_head.Sopversion = request.SOPVersion;
                        sop_head.UpdatedBy = _currentUser.UserId;
                        sop_head.Status = 1;
                        sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                       var updatedId= await _sopRepository.UPdateSOPHeader(sop_head);

                       await _sopRepository.LinkSOPWithIncident(updatedId, request.IncidentID, _currentUser.UserId,_currentUser.TimeZone);
                        Rt_SopHeaderId = updatedId;
                        ReminderCount = sop_head.ReminderCount;
                    }
                }
                else
                {
                    Sopheader sop_head = new Sopheader();
                    sop_head.ReviewDate = request.ReviewDate;
                    sop_head.Sopowner = request.SOPOwner;
                    sop_head.ReviewFrequency = request.ReviewFrequency;
                    sop_head.CompanyId = _currentUser.CompanyId;
                    sop_head.Sopversion = request.SOPVersion;
                    sop_head.CreatedBy = _currentUser.UserId;
                    sop_head.CreatedOn = DateTime.Now.GetDateTimeOffset( _currentUser.TimeZone);
                    sop_head.Status = 1;
                    sop_head.ReminderCount = ReminderCount;
                    sop_head.UpdatedBy = _currentUser.UserId;
                    sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset( _currentUser.TimeZone);
                    var NewSopheaderID = await _sopRepository.SaveSOPHeader(sop_head);

                    await _sopRepository.LinkSOPWithIncident(NewSopheaderID, request.IncidentID, _currentUser.UserId, _currentUser.TimeZone);

                    Rt_SopHeaderId = NewSopheaderID;
                    //using (var dbContextTransaction = db.Database.BeginTransaction())
                    //{

                        int TextTransacTypeId =await _sopRepository.GetTransactionTypeID("ISOPUSAGE");

                        decimal VATRate = 20M;
                      var profile = await _sopRepository.GetCompanyProfile(_currentUser.CompanyId);
                        VATRate = (decimal)profile.VATRate;

                        decimal VatValue = (profile.SOPTokenValue * VATRate) / 100;
                        decimal iSOPNetValue = profile.SOPTokenValue - VatValue;

                   await  _sopRepository.UpdateTransactionDetails(0, _currentUser.CompanyId, TextTransacTypeId, iSOPNetValue, iSOPNetValue, 1, iSOPNetValue, iSOPNetValue, VatValue, profile.SOPTokenValue,
                            Rt_SopHeaderId, DateTime.Now, _currentUser.UserId, "SOPDOC" + Rt_SopHeaderId.ToString(), _currentUser.TimeZone);

                       await _usagecalc.update_company_balance(_currentUser.CompanyId, profile.SOPTokenValue);
                  
                }
                if (request.Status != 1)
                {
                    DBC.DeleteScheduledJob("SOP_REVIEW_" + Rt_SopHeaderId, "REVIEW_REMINDER");
                }
                else
                {
                   await _sopRepository.CreateSOPReviewReminder(request.IncidentID, Rt_SopHeaderId, _currentUser.CompanyId, request.ReviewDate, request.ReviewFrequency, ReminderCount);
                }

                //Creating the contents for breif description
                //int rtContentSectionID = AU_ContentSection(ContentSectionID, Rt_SopHeaderId, "BRIEF_DESCRIPTION", 1, "INFO", 0);
                //int rtContentID = AU_Content(ContentID, BriefDescription, "INFO", 1);
                //AU_SOPDetail(Rt_SopHeaderId, rtContentID, rtContentSectionID);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
