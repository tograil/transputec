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
using Microsoft.AspNetCore.Http;
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
        private readonly DBCommon _DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Messaging _MSG;
        private readonly SendEmail _SDE;
        public CompanyParametersRepository(CrisesControlContext context, ILogger<CompanyParametersRepository> logger, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            _DBC = new DBCommon(context, _httpContextAccessor);
            _MSG = new Messaging(context, _httpContextAccessor);
            _SDE = new SendEmail(context, _DBC);
        }
        public async Task<IEnumerable<CascadingPlanReturn>> GetCascading(int planID, string planType, int companyId, bool getDetails = false) {
            try {

                var pCompanyID = new SqlParameter("@CompanyId", companyId);
                var pPlanType = new SqlParameter("@PlanType", planType);
                var pPlanID = new SqlParameter("@PlanId", planID);
                var response = await _context.Set<CascadingPlanReturn>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan @CompanyId, @PlanType, @PlanId", pCompanyID, pPlanType, pPlanID).ToListAsync();
                if (response != null) {
                    if (planID > 0) {
                        var singlersp = response.FirstOrDefault();
                        singlersp.CommsMethod = GetCascadingDetails(singlersp.PlanID, companyId);
                        return response;
                    } else if (getDetails) {
                        response.Select(c => {
                            c.CommsMethod = GetCascadingDetails(c.PlanID, companyId);
                            return c;
                        });
                    }
                    return response;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CascadingPlanReturn>();
        }
        public List<CommsMethodPriority> GetCascadingDetails(int planID, int companyId) {
            try {

                var pPlanID = new SqlParameter("@PlanId", planID);
                var pCompanyID = new SqlParameter("@CompanyId", companyId);
                var cascadingPlans = _context.Set<CommsMethodPriority>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan_Details @PlanId, @CompanyId", pCompanyID, pPlanID).ToList();
                return cascadingPlans;

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CommsMethodPriority>();
        }

        public async Task<IEnumerable<CompanyFtp>> GetCompanyFTP(int companyID) {
            try {

                var CompanyId = new SqlParameter("@CompanyID", companyID);
                return await _context.Set<CompanyFtp>().FromSqlRaw("EXEC Pro_Get_Company_FTP @CompanyID", CompanyId).ToListAsync();
            } catch (Exception ex) {
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
        public async Task<string> GetCompanyParameter(string key, int companyId, string Default = "", string customerId = "") {
            try {
                key = key.ToUpper();

                if (companyId > 0) {
                    var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == key && CP.CompanyId == companyId).FirstOrDefaultAsync();
                    if (LKP != null) {
                        Default = LKP.Value;
                    } else {

                        var LPR = await _context.Set<LibCompanyParameter>().Where(CP => CP.Name == key).FirstOrDefaultAsync();
                        if (LPR != null) {
                            Default = LPR.Value;
                        } else {
                            Default = _DBC.LookupWithKey(key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key)) {

                    var cmp = await _context.Set<Company>().Where(w => w.CustomerId == customerId).FirstOrDefaultAsync();
                    if (cmp != null) {
                        var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == key && CP.CompanyId == cmp.CompanyId).FirstOrDefaultAsync();
                        if (LKP != null) {
                            Default = LKP.Value;
                        }
                    } else {
                        Default = "NOT_EXIST";
                    }
                }

                return Default;
            } catch (Exception ex) {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }
        public async Task<Result> SaveCompanyFTP(int companyId, string hostName, string userName, string securityKey, string protocol,
    int port, string remotePath, string logonType, bool deleteSourceFile, string shaFingerPrint) {
            try {
                var pCompanyId = new SqlParameter("@CompanyID", companyId);
                var pHostName = new SqlParameter("@HostName", hostName);
                var pUserName = new SqlParameter("@UserName", userName);
                var pProtocol = new SqlParameter("@Protocol", protocol);
                var pSecurityKey = new SqlParameter("@SecurityKey", securityKey);
                var pLogonType = new SqlParameter("@LogonType", logonType);
                var pPort = new SqlParameter("@Port", port);
                var pDeleteSourceFile = new SqlParameter("@DeleteSourceFile", deleteSourceFile);
                var pRemotePath = new SqlParameter("@RemotePath", remotePath);
                var pSHAFingerPrint = new SqlParameter("@SHAFingerPrint", shaFingerPrint);

                var result = _context.Set<Result>().FromSqlRaw("exec Pro_Save_Company_FTP @CompanyID,@HostName,@UserName,@Protocol,@SecurityKey,@LogonType,@Port,@DeleteSourceFile,@RemotePath,@SHAFingerPrint",
                                    pCompanyId, pHostName, pUserName, pProtocol, pSecurityKey, pLogonType, pPort, pDeleteSourceFile, pRemotePath, pSHAFingerPrint).AsEnumerable();

                if (result != null) {
                    var intResult = result.FirstOrDefault();
                    return intResult;
                }
                return null;

            } catch (Exception ex) {
                throw new CompanyNotFoundException(companyId, 0);
            }

        }
        public async Task<bool> SaveCascading(int planID, string planName, string planType, bool launchSOS, int launchSOSInterval, List<CommsMethodPriority> commsMethod, int companyID) {
            try {
                int planId = await SaveCascadingPlanHeader(planID, planName, planType, launchSOS, launchSOSInterval, companyID);
                if (planId > 0) {
                    await SaveCascadingDetails(planId, commsMethod, companyID);
                    return true;
                }
                return false;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> SaveCascadingPlanHeader(int planID, string planName, string planType, bool launchSOS, int launchSOSInterval, int companyID) {
            try {
                if (planID > 0) {
                    var cascade = await _context.Set<CascadingPlan>().Where(CP => CP.PlanId == planID && CP.CompanyId == companyID).FirstOrDefaultAsync();
                    if (cascade != null) {
                        cascade.PlanName = planName;
                        cascade.PlanType = planType;
                        cascade.LaunchSos = launchSOS;
                        cascade.LaunchSosinterval = launchSOSInterval;
                        _context.Update(cascade);
                        await _context.SaveChangesAsync();
                    }
                } else {
                    CascadingPlan CP = new CascadingPlan() {
                        CompanyId = companyID,
                        LaunchSos = launchSOS,
                        LaunchSosinterval = launchSOSInterval,
                        PlanName = planName,
                        PlanType = planType
                    };
                    await _context.AddAsync(CP);
                    await _context.SaveChangesAsync();
                    planID = CP.PlanId;
                }

                return planID;
            } catch (Exception ex) {
                throw ex;
                return 0;
            }

        }

        public async Task SaveCascadingDetails(int planID, List<CommsMethodPriority> commsMethod, int companyID) {
            try {
                var PIDel = await _context.Set<PriorityInterval>()
                             .Where(PI => PI.CascadingPlanId == planID
                             ).ToListAsync();

                _context.RemoveRange(PIDel);
                await _context.SaveChangesAsync();

                foreach (CommsMethodPriority PObj in commsMethod) {
                    PriorityInterval PI = new PriorityInterval() {
                        CascadingPlanId = planID,
                        CompanyId = companyID,
                        MessageType = PObj.MessageType,
                        Interval = PObj.Interval,
                        Priority = PObj.Priority,
                        Methods = string.Join(",", PObj.Methods)
                    };
                    await _context.AddAsync(PI);
                }
                await _context.SaveChangesAsync();

                UpdateCascadingAsync(companyID);
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<bool> SavePriority(string paramName, bool enableSetting, List<CommsMethodPriority> commsMethod, PriorityLevel pingPriority, PriorityLevel incidentPriority,
            SeverityLevel incidentSeverity, string type, int userID, int companyID, string timeZoneId) {
            try {

                var cascade = await _context.Set<CompanyParameter>().Where(w => w.Name == paramName && w.CompanyId == companyID).FirstOrDefaultAsync();
                if (cascade != null) {
                    cascade.Value = enableSetting ? "true" : "false";
                    cascade.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                    cascade.UpdatedBy = userID;
                    _context.Update(cascade);
                    await _context.SaveChangesAsync();
                }


                if (paramName == "ALLOW_CHANNEL_PRIORITY_SETUP" || paramName == "ALLOW_CHANNEL_SEVERITY_SETUP") {

                    var prioritytmpl = await _context.Set<PriorityMethod>().Where(PM => PM.CompanyId == companyID).ToListAsync();
                    if (pingPriority != null) {
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 100).Select(s => s).FirstOrDefault().Methods = string.Join(",", pingPriority.PriorityLow);
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 500).Select(s => s).FirstOrDefault().Methods = string.Join(",", pingPriority.PriorityMed);
                        prioritytmpl.Where(w => w.MessageType == "Ping" && w.PriorityLevel == 999).Select(s => s).FirstOrDefault().Methods = string.Join(",", pingPriority.PriorityHigh);
                    }

                    if (incidentPriority != null) {
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 100).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentPriority.PriorityLow);
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 500).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentPriority.PriorityMed);
                        prioritytmpl.Where(w => w.MessageType == "Incident" && w.PriorityLevel == 999).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentPriority.PriorityHigh);
                    }

                    if (incidentSeverity != null) {
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 1).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentSeverity.Severity1);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 2).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentSeverity.Severity2);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 3).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentSeverity.Severity3);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 4).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentSeverity.Severity4);
                        prioritytmpl.Where(w => w.MessageType == "IncidentSeverity" && w.PriorityLevel == 5).Select(s => s).FirstOrDefault().Methods = string.Join(",", incidentSeverity.Severity5);
                    }
                    // _context.Update(prioritytmpl);
                    await _context.SaveChangesAsync();
                }

                return true;
            } catch (Exception ex) {
                throw ex;
                return false;
            }
        }
        public void UpdateCascadingAsync(int companyID) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                _context.Set<Result>().FromSqlRaw("exec Pro_Update_Cascading_Channel @CompanyID", pCompanyID).AsEnumerable();

            } catch (Exception ex) {
                throw ex;
            }
        }

        public void UpdateOffDuty(int companyID) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                _context.Set<Result>().FromSqlRaw("exec Pro_Update_OffDuty @CompanyID", pCompanyID).FirstOrDefault();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<bool> CompanyDataReset(string[] resetOptions, int companyID, string timeZoneId) {
            try {
                foreach (string option in resetOptions) {
                    if (option.ToUpper() == "PINGS") {
                        ResetPings(companyID);
                    } else if (option.ToUpper() == "ACTIVEINCIDENT") {
                        ResetActiveIncident(companyID);
                    } else if (option.ToUpper() == "GLOBALCONFIG") {
                        ResetGlobalConfig(companyID, timeZoneId);
                    }
                    return true;
                }
            } catch (Exception ex) {
                throw ex;
            }
            return false;
        }

        public void ResetGlobalConfig(int companyID, string timeZoneId) {
            try {
                DateTimeOffset CreatedNow = DateTime.Now.GetDateTimeOffset(timeZoneId);
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                var pCreatedNow = new SqlParameter("@CreatedOnOffset", CreatedNow);
                _context.Set<Result>().FromSqlRaw("Pro_DC_Global_Config @CompanyID, @CreatedOnOffset", pCompanyID, pCreatedNow).FirstOrDefault();
            } catch (Exception ex) {
                throw ex;
            }
        }
        public void ResetPings(int companyID) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                _context.Set<Result>().FromSqlRaw("Pro_DC_Ping @CompanyID", pCompanyID).FirstOrDefault();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void ResetActiveIncident(int companyID) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                _context.Set<Result>().FromSqlRaw("Pro_DC_Active_Incident @CompanyID", pCompanyID).FirstOrDefault();
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<bool> DeleteCascading(int planID, int companyId, int userId) {

            try {
                var cascading = await _context.Set<CascadingPlan>()
                                 .Where(CP => CP.CompanyId == companyId && CP.PlanId == planID
                                 ).FirstOrDefaultAsync();
                if (cascading != null) {
                    _context.Remove(cascading);
                    var plansteps = await _context.Set<PriorityInterval>()
                                     .Where(STP => STP.CascadingPlanId == planID
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
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<bool> SaveParameter(int parameterID, string parameterName, string parameterValue, int currentUserID, int companyID, string timeZoneId) {
            try {
                if (parameterID > 0) {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP => CP.CompanyId == companyID && CP.CompanyParametersId == parameterID
                                           ).FirstOrDefaultAsync();

                    if (CompanyParameter != null) {
                        if (parameterValue != null) {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI") {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(parameterValue) * 60);
                            } else {
                                CompanyParameter.Value = parameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                        CompanyParameter.UpdatedBy = currentUserID;
                        _context.Update(CompanyParameter);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                } else if (!string.IsNullOrEmpty(parameterName)) {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP => CP.CompanyId == companyID && CP.Name == parameterName).FirstOrDefaultAsync();

                    if (CompanyParameter != null) {
                        if (parameterValue != null) {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI") {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(parameterValue) * 60);
                            } else {
                                CompanyParameter.Value = parameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                        CompanyParameter.UpdatedBy = currentUserID;
                        _context.Update(CompanyParameter);
                        await _context.SaveChangesAsync();
                        return true;
                    } else {
                        int CompanyParametersId = await AddCompanyParameter(parameterName, parameterValue, companyID, currentUserID, timeZoneId);
                        return true;
                    }
                }

                if (parameterName.ToUpper() == "RECHARGE_BALANCE_TRIGGER") {
                    var profile = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == companyID).FirstOrDefaultAsync();
                    if (profile != null) {
                        profile.MinimumBalance = Convert.ToDecimal(parameterValue);
                        _context.Update(profile);
                        await _context.SaveChangesAsync();
                        return true;

                    }
                }
                return false;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<int> AddCompanyParameter(string name, string value, int companyId, int currentUserId, string timeZoneId) {
            try {
                var comp_param = await _context.Set<CompanyParameter>().Where(CP => CP.CompanyId == companyId && CP.Name == name).AnyAsync();
                if (!comp_param) {
                    CompanyParameter NewCompanyParameters = new CompanyParameter() {
                        CompanyId = companyId,
                        Name = name,
                        Value = value,
                        Status = 1,
                        CreatedBy = currentUserId,
                        UpdatedBy = currentUserId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
                    };
                    await _context.AddAsync(NewCompanyParameters);
                    await _context.SaveChangesAsync();
                    return NewCompanyParameters.CompanyParametersId;
                }
            } catch (Exception ex) {
                throw ex;
            }
            return 0;
        }
        public void SetSSOParameters(int companyId) {
            try {
                var pCompanyID = new SqlParameter("@CompanyId", companyId);
                _context.Set<JsonResults>().FromSqlRaw("exec Pro_Configure_SSO @CompanyId", pCompanyID).AsEnumerable();

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<OTPResponse> SegregationOtp(int companyId, int currentUserId, string method) {
            OTPResponse result = new OTPResponse();
            try {
                var reg_user = await (from U in _context.Set<User>() where U.UserId == currentUserId && U.CompanyId == companyId select U).FirstOrDefaultAsync();
                if (reg_user != null) {
                    if (reg_user.RegisteredUser) {

                        string OTPMessage = _DBC.LookupWithKey("SEGREGATION_CODE_MSG");

                        CommsHelper CH = new CommsHelper(_context, _httpContextAccessor);

                        result.Data = CH.SendOTP(reg_user.Isdcode, reg_user.Isdcode + reg_user.MobileNo, OTPMessage, "SEGREGATION", method.ToUpper());
                    } else {
                        result.ErrorId = 234;
                        result.ErrorCode = "E234";
                        result.Message = "You are not authorized to make this request";
                    }
                }
            } catch (Exception ex) {
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
