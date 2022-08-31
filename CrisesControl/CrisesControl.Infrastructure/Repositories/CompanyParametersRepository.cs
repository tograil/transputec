using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company = CrisesControl.Core.Companies.Company;
using CompanyParameter = CrisesControl.Core.Companies.CompanyParameter;

namespace CrisesControl.Infrastructure.Repositories {
    public class CompanyParametersRepository : ICompanyParametersRepository {
        private readonly CrisesControlContext _context;
        private readonly ILogger<CompanyParametersRepository> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly DBCommon _DBC;
        private readonly CommsHelper _CH;
        public CompanyParametersRepository(CrisesControlContext context, ILogger<CompanyParametersRepository> logger, DBCommon DBC, CommsHelper CH)
        {
            this._context = context;
            _DBC = DBC;
            _CH = CH;
        }
        public async Task<IEnumerable<CascadingPlanReturn>> GetCascading(int PlanID, string PlanType, int CompanyId, bool GetDetails = false)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                var pPlanType = new SqlParameter("@PlanType", PlanType);
                var pPlanID = new SqlParameter("@PlanId", PlanID);
               var response = await _context.Set<CascadingPlanReturn>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan @CompanyId, @PlanType, @PlanId",pCompanyID, pPlanType, pPlanID).ToListAsync();
                if (response != null)
                {
                    if (PlanID > 0)
                    {
                        var singlersp =  response.FirstOrDefault();
                        singlersp.CommsMethod = GetCascadingDetails(singlersp.PlanID, CompanyId);
                        return response;
                    }
                    else if (GetDetails)
                    {
                        response.Select(c => {
                            c.CommsMethod =GetCascadingDetails(c.PlanID, CompanyId);
                            return c;
                        });
                    }
                }

             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CascadingPlanReturn>();
        }
        public List<CommsMethodPriority> GetCascadingDetails(int PlanID,  int CompanyId )
        {
            try
            {

                var pPlanID = new SqlParameter("@PlanId", PlanID);
                var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                var cascadingPlans =  _context.Set<CommsMethodPriority>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan_Details @PlanId, @CompanyId", pCompanyID,  pPlanID).ToList();
                return cascadingPlans;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CommsMethodPriority>();
        }

        public async Task<IEnumerable<CompanyFtp>> GetCompanyFTP(int CompanyID)
        {
            try
            {
             
                var CompanyId = new SqlParameter("@CompanyID", CompanyID);
                return await _context.Set<CompanyFtp>().FromSqlRaw("EXEC Pro_Get_Company_FTP @CompanyID",  CompanyId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);

                var result = await _context.Set<CompanyParameterItem>().FromSqlRaw("exec Pro_Company_GetAllCompanyParameters {0}", pCompanyID).ToListAsync();
                return result;
            } catch (Exception ex) {
                return null;
            }

        }
        public async Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "")
        {
            try
            {
                Key = Key.ToUpper();

                if (CompanyId > 0)
                {
                    var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                    else
                    {

                        var LPR = await _context.Set<LibCompanyParameter>().Where(CP => CP.Name == Key).FirstOrDefaultAsync();
                        if (LPR != null)
                        {
                            Default = LPR.Value;
                        }
                        else
                        {
                            Default = await _messageRepository.LookupWithKey(Key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(CustomerId) && !string.IsNullOrEmpty(Key))
                {

                    var cmp = await _context.Set<Company>().Where(w => w.CustomerId == CustomerId).FirstOrDefaultAsync();
                    if (cmp != null)
                    {
                        var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == cmp.CompanyId).FirstOrDefaultAsync();
                        if (LKP != null)
                        {
                            Default = LKP.Value;
                        }
                    }
                    else
                    {
                        Default = "NOT_EXIST";
                    }
                }

                return Default;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }
        public async Task<Result> SaveCompanyFTP(int CompanyId, string HostName, string UserName, string SecurityKey, string Protocol,
    int Port, string RemotePath, string LogonType, bool DeleteSourceFile, string SHAFingerPrint)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
                var pHostName = new SqlParameter("@HostName", HostName);
                var pUserName = new SqlParameter("@UserName", UserName);
                var pProtocol = new SqlParameter("@Protocol", Protocol);
                var pSecurityKey = new SqlParameter("@SecurityKey", SecurityKey);
                var pLogonType = new SqlParameter("@LogonType", LogonType);
                var pPort = new SqlParameter("@Port", Port);
                var pDeleteSourceFile = new SqlParameter("@DeleteSourceFile", DeleteSourceFile);
                var pRemotePath = new SqlParameter("@RemotePath", RemotePath);
                var pSHAFingerPrint = new SqlParameter("@SHAFingerPrint", SHAFingerPrint);

                var result =  _context.Set<Result>().FromSqlRaw("exec Pro_Save_Company_FTP @CompanyID,@HostName,@UserName,@Protocol,@SecurityKey,@LogonType,@Port,@DeleteSourceFile,@RemotePath,@SHAFingerPrint",
                                    pCompanyId, pHostName, pUserName, pProtocol, pSecurityKey, pLogonType, pPort, pDeleteSourceFile, pRemotePath, pSHAFingerPrint).AsEnumerable();
                
                if (result != null)
                {
                    var intResult = result.FirstOrDefault();
                    return intResult;
                }
                return null;
                
            }
            catch (Exception ex)
            {
                throw new CompanyNotFoundException(CompanyId, 0);
            }
        
        }
        public async Task<bool> SaveCascading(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, List<CommsMethodPriority> CommsMethod, int CompanyID)
        {
            try
            {
                int planId =await SaveCascadingPlanHeader(PlanID, PlanName, PlanType, LaunchSOS, LaunchSOSInterval, CompanyID);
                if (planId > 0)
                {
                   await SaveCascadingDetails(planId, CommsMethod, CompanyID);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveCascadingPlanHeader(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, int CompanyID)
        {
            try
            {
                if (PlanID > 0)
                {
                    var cascade = await _context.Set<CascadingPlan>().Where(CP=> CP.PlanId == PlanID && CP.CompanyId == CompanyID).FirstOrDefaultAsync();
                    if (cascade != null)
                    {
                        cascade.PlanName = PlanName;
                        cascade.PlanType = PlanType;
                        cascade.LaunchSos = LaunchSOS;
                        cascade.LaunchSosinterval = LaunchSOSInterval;
                        _context.Update(cascade);
                       await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    CascadingPlan CP = new CascadingPlan()
                    {
                        CompanyId = CompanyID,
                        LaunchSos = LaunchSOS,
                        LaunchSosinterval = LaunchSOSInterval,
                        PlanName = PlanName,
                        PlanType = PlanType
                    };
                   await _context.AddAsync(CP);
                   await _context.SaveChangesAsync();
                    PlanID = CP.PlanId;
                }

                return PlanID;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
            
        }

        public async Task SaveCascadingDetails(int PlanID, List<CommsMethodPriority> CommsMethod, int CompanyID)
        {
            try
            {
                var PIDel = await _context.Set<PriorityInterval>()
                             .Where(PI=> PI.CascadingPlanId == PlanID
                             ).ToListAsync();

                _context.RemoveRange(PIDel);
              await  _context.SaveChangesAsync();

                foreach (CommsMethodPriority PObj in CommsMethod)
                {
                    PriorityInterval PI = new PriorityInterval()
                    {
                        CascadingPlanId = PlanID,
                        CompanyId = CompanyID,
                        MessageType = PObj.MessageType,
                        Interval = PObj.Interval,
                        Priority = PObj.Priority,
                        Methods = string.Join(",", PObj.Methods)
                    };
                   await _context.AddAsync(PI);
                }
               await _context.SaveChangesAsync();

               await UpdateCascadingAsync(CompanyID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SavePriority(string ParamName, bool EnableSetting, List<CommsMethodPriority> CommsMethod, PriorityLevel PingPriority, PriorityLevel IncidentPriority,
            SeverityLevel IncidentSeverity, string Type, int UserID, int CompanyID, string TimeZoneId)
        {
            try
            {

                var cascade = await _context.Set<CompanyParameter>().Where(w => w.Name == ParamName && w.CompanyId == CompanyID).FirstOrDefaultAsync();
                if (cascade != null)
                {
                    cascade.Value = EnableSetting ? "true" : "false";
                    cascade.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                    cascade.UpdatedBy = UserID;
                    _context.Update(cascade);
                   await _context.SaveChangesAsync();
                }

               
                if (ParamName == "ALLOW_CHANNEL_PRIORITY_SETUP" || ParamName == "ALLOW_CHANNEL_SEVERITY_SETUP")
                {

                    var prioritytmpl = await _context.Set<PriorityMethod>().Where(PM=> PM.CompanyId == CompanyID ).ToListAsync();
                    if (PingPriority != null)
                    {
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 100).Select(s => s).FirstOrDefault().Methods = string.Join(",", PingPriority.PriorityLow);
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 500).Select(s => s).FirstOrDefault().Methods = string.Join(",", PingPriority.PriorityMed);
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 999).Select(s => s).FirstOrDefault().Methods = string.Join(",", PingPriority.PriorityHigh);
                    }

                    if (IncidentPriority != null)
                    {
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 100).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentPriority.PriorityLow);
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 500).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentPriority.PriorityMed);
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 999).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentPriority.PriorityHigh);
                    }

                    if (IncidentSeverity != null)
                    {
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 1).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentSeverity.Severity1);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 2).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentSeverity.Severity2);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 3).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentSeverity.Severity3);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 4).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentSeverity.Severity4);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 5).Select(s => s).FirstOrDefault().Methods = string.Join(",", IncidentSeverity.Severity5);
                    }
                   // _context.Update(prioritytmpl);
                   await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task UpdateCascadingAsync(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
             _context.Set<Result>().FromSqlRaw("exec Pro_Update_Cascading_Channel @CompanyID", pCompanyID).AsEnumerable();
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateOffDuty(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
               await _context.Set<Result>().FromSqlRaw("exec Pro_Update_OffDuty @CompanyID", pCompanyID).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CompanyDataReset(string[] ResetOptions, int CompanyID, string TimeZoneId)
        {
            try
            {
                foreach (string option in ResetOptions)
                {
                    if (option.ToUpper() == "PINGS")
                    {
                      await  ResetPings(CompanyID);
                    }
                    else if (option.ToUpper() == "ACTIVEINCIDENT")
                    {
                       await ResetActiveIncident(CompanyID);
                    }
                    else if (option.ToUpper() == "GLOBALCONFIG")
                    {
                       await ResetGlobalConfig(CompanyID, TimeZoneId);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task ResetGlobalConfig(int CompanyID, string TimeZoneId)
        {
            try
            {
                DateTimeOffset CreatedNow = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pCreatedNow = new SqlParameter("@CreatedOnOffset", CreatedNow);
                await _context.Set<Result>().FromSqlRaw("Pro_DC_Global_Config @CompanyID, @CreatedOnOffset", pCompanyID, pCreatedNow).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ResetPings(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
               await _context.Set<Result>().FromSqlRaw("Pro_DC_Ping @CompanyID", pCompanyID).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ResetActiveIncident(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
               await _context.Set<Result>().FromSqlRaw("Pro_DC_Active_Incident @CompanyID", pCompanyID).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteCascading(int PlanID, int CompanyId, int UserId)
        {
            
            try
            {
                var cascading = await _context.Set<CascadingPlan>()
                                 .Where(CP=> CP.CompanyId == CompanyId && CP.PlanId == PlanID
                                 ).FirstOrDefaultAsync();
                if (cascading != null)
                {
                    _context.Remove(cascading);
                    var plansteps = await _context.Set<PriorityInterval>()
                                     .Where(STP=> STP.CascadingPlanId == PlanID
                                     ).ToListAsync();
                    _context.RemoveRange(plansteps);
                    await _context.SaveChangesAsync();
                    return true;
                }
                //else
                //{
                //    ResultDTO.ErrorId = 110;
                //    ResultDTO.Message = "No record found.";
                    
                //}
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SaveParameter(int ParameterID, string ParameterName, string ParameterValue, int CurrentUserID, int CompanyID, string TimeZoneId)
        {
            try
            {
                if (ParameterID > 0)
                {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP=> CP.CompanyId == CompanyID && CP.CompanyParametersId == ParameterID
                                           ).FirstOrDefaultAsync();

                    if (CompanyParameter != null)
                    {
                        if (ParameterValue != null)
                        {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI")
                            {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(ParameterValue) * 60);
                            }
                            else
                            {
                                CompanyParameter.Value = ParameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        CompanyParameter.UpdatedBy = CurrentUserID;
                        _context.Update(CompanyParameter);
                       await _context.SaveChangesAsync();
                        return true;
                    }
                }
                else if (!string.IsNullOrEmpty(ParameterName))
                {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP=> CP.CompanyId == CompanyID && CP.Name == ParameterName).FirstOrDefaultAsync();

                    if (CompanyParameter != null)
                    {
                        if (ParameterValue != null)
                        {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI")
                            {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(ParameterValue) * 60);
                            }
                            else
                            {
                                CompanyParameter.Value = ParameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                        CompanyParameter.UpdatedBy = CurrentUserID;
                        _context.Update(CompanyParameter);
                       await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        int CompanyParametersId =await AddCompanyParameter(ParameterName, ParameterValue, CompanyID, CurrentUserID, TimeZoneId);
                        return true;
                    }
                }

                if (ParameterName.ToUpper() == "RECHARGE_BALANCE_TRIGGER")
                {
                    var profile = await _context.Set<CompanyPaymentProfile>().Where(CP=> CP.CompanyId == CompanyID).FirstOrDefaultAsync();
                    if (profile != null)
                    {
                        profile.MinimumBalance = Convert.ToDecimal(ParameterValue);
                        _context.Update(profile);
                       await _context.SaveChangesAsync();
                        return true;

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId)
        {
            try
            {
                var comp_param = await _context.Set<CompanyParameter>().Where(CP=> CP.CompanyId == CompanyId && CP.Name == Name).AnyAsync();
                if (!comp_param)
                {
                    CompanyParameter NewCompanyParameters = new CompanyParameter()
                    {
                        CompanyId = CompanyId,
                        Name = Name,
                        Value = Value,
                        Status = 1,
                        CreatedBy = CurrentUserId,
                        UpdatedBy = CurrentUserId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                    };
                    await _context.AddAsync(NewCompanyParameters);
                    await _context.SaveChangesAsync();
                    return NewCompanyParameters.CompanyParametersId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }
        public async Task SetSSOParameters(int CompanyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                _context.Set<JsonResults>().FromSqlRaw("exec Pro_Configure_SSO @CompanyId", pCompanyID).AsEnumerable();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OTPResponse> SegregationOtp(int companyId, int currentUserId, string method)
        {
            OTPResponse result = new OTPResponse();
            try
            {
                var reg_user = await (from U in _context.Set<User>() where U.UserId == currentUserId && U.CompanyId == companyId select U).FirstOrDefaultAsync();
                if (reg_user != null)
                {
                    if (reg_user.RegisteredUser)
                    {

                        string OTPMessage = _DBC.LookupWithKey("SEGREGATION_CODE_MSG");


                        result.Data =  _CH.SendOTP(reg_user.Isdcode, reg_user.Isdcode + reg_user.MobileNo, OTPMessage, "SEGREGATION", method.ToUpper());
                    }
                    else
                    {
                        result.ErrorId = 234;
                        result.ErrorCode = "E234";
                        result.Message = "You are not authorized to make this request";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        //public int AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId)
        //{
        //    try
        //    {
        //        var comp_param = (from CP in db.CompanyParameters where CP.CompanyId == CompanyId && CP.Name == Name select CP).Any();
        //        if (!comp_param)
        //        {
        //            CompanyParameters NewCompanyParameters = new CompanyParameters()
        //            {
        //                CompanyId = CompanyId,
        //                Name = Name,
        //                Value = Value,
        //                Status = 1,
        //                CreatedBy = CurrentUserId,
        //                UpdatedBy = CurrentUserId,
        //                CreatedOn = DateTime.Now,
        //                UpdatedOn = GetDateTimeOffset(DateTime.Now, TimeZoneId)
        //            };
        //            db.CompanyParameters.Add(NewCompanyParameters);
        //            db.SaveChanges(CurrentUserId, CompanyId);
        //            return NewCompanyParameters.CompanyParametersId;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        catchException(ex);
        //    }
        //    return 0;
        //}


    }
}
