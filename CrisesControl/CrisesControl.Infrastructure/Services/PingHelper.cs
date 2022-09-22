using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Import;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class PingHelper
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Messaging _MSG;
        private readonly DBCommon _DBC;
        private readonly QueueConsumer queue;
        private readonly QueueHelper _queueHelper;

        public bool IsFundAvailable = true;
        public PingHelper(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _DBC = new DBCommon(_context,_httpContextAccessor);
            _MSG = new Messaging(_context,_httpContextAccessor,_DBC);
            queue = new QueueConsumer(_context, _httpContextAccessor);
            _queueHelper = new QueueHelper(_context);
        }

        public List<PublicAlertRtn> GetPublicAlert(int companyId, int targetUserId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pTargetUserId = new SqlParameter("@UserID", targetUserId);

                var result = _context.Set<PublicAlertRtn>().FromSqlRaw("EXEC Public_Alert_Get @CompanyID, @UserID",
                    pCompanyID, pTargetUserId).ToList().Select(c => {
                        c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return new List<PublicAlertRtn>();
            }
        }

        public dynamic GetPublicAlertTemplate(int templateId, int userId, int companyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pTemplateID = new SqlParameter("@TemplateID", templateId);
                var pUserID = new SqlParameter("@UserID", userId);

                var result = _context.Set<PublicAlertTemplate>().FromSqlRaw("EXEC Public_Alert_Template_Get @CompanyID, @TemplateID, @UserID", pCompanyID, pTemplateID, pUserID).ToList();

                if (templateId > 0 && result != null)
                {
                    return result.FirstOrDefault();
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> PingMessages(int CompanyId, string MessageText, List<AckOption> AckOptions, int Priority, bool MultiResponse, string MessageType,
       int IncidentActivationId, int CurrentUserId, string TimeZoneId, PingMessageObjLst[] PingMessageObjLst, int[] UsersToNotify, int AssetId = 0,
       bool SilentMessage = false, int[] MessageMethod = null, List<MediaAttachment> MediaAttachments = null, List<string> SocialHandle = null,
       int CascadePlanID = 0)
        {
            bool NotifyKeyholders = false;
            int MessageId = 0;
            int Source = 1;
            try
            {
                _MSG.TimeZoneId = TimeZoneId;
                _MSG.CascadePlanID = CascadePlanID;
                if (MessageType == "EventLogNotify")
                {
                    _MSG.MessageSourceAction = SourceAction.EventLogNotify;
                    MessageType = "Ping";
                    Source = 7;
                }
                else
                {
                    _MSG.MessageSourceAction = IncidentActivationId > 0 ? SourceAction.IncidentUpdate : SourceAction.Ping;
                }

                int tblmessageid = await _MSG.CreateMessage(CompanyId, MessageText, MessageType, IncidentActivationId, Priority, CurrentUserId,
                    Source, _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId), MultiResponse, AckOptions, 99, AssetId, 0, false, SilentMessage,
                    MessageMethod, MediaAttachments);

                MessageId = tblmessageid;

                List<PingMessageObjLst> LstIncNotiLst = new List<PingMessageObjLst>(PingMessageObjLst);
                //Checking whether to notifiy keyholders not not
                if (MessageType.ToUpper() == "PING")
                {
                    if (PingMessageObjLst.Count() > 0 && MessageType.ToUpper() == "PING")
                    {
                        foreach (var INotiLst in LstIncNotiLst)
                        {
                            if (INotiLst.ObjectMappingId > 0)
                            {
                                CreateAdHocNotificationList(tblmessageid, INotiLst.ObjectMappingId, INotiLst.SourceObjectPrimaryId, CompanyId, CurrentUserId, TimeZoneId);
                            }
                        }
                    }
                }
                else if (MessageType.ToUpper() == "INCIDENT")
                {
                    if (PingMessageObjLst.Count() > 0 && LstIncNotiLst != null)
                    {
                        foreach (var INotiLst in LstIncNotiLst)
                        {
                            if (INotiLst.ObjectMappingId > 0)
                            {
                                await _MSG.CreateIncidentNotificationList(tblmessageid, IncidentActivationId, INotiLst.ObjectMappingId, INotiLst.SourceObjectPrimaryId, CurrentUserId, CompanyId, TimeZoneId);
                            }
                        }
                    }
                }

                NotifyKeyholders = Convert.ToBoolean(_DBC.GetCompanyParameter("NOTIFY_KEYHOLDER_BY_PING", CompanyId));

                if (NotifyKeyholders)
                {
                    var roles = _DBC.CCRoles(true);
                    var UserLst = (from U in _context.Set<User>()
                                   where roles.Contains(U.UserRole)
                                   && U.CompanyId == CompanyId && U.Status == 1
                                   select U.UserId).Distinct().ToList();

                    _MSG.AddUserToNotify(tblmessageid, UserLst.Distinct().ToList());
                }

                if (UsersToNotify != null)
                {
                    _MSG.AddUserToNotify(tblmessageid, UsersToNotify.ToList(), IncidentActivationId);
                }

                if (SocialHandle != null)
                    Task.Factory.StartNew(() => _MSG.SocialPosting(tblmessageid, SocialHandle, CompanyId));

                //QueueHelper.MessageListQueue(tblmessageid);
                await queue.CreateMessageList(tblmessageid);
                IsFundAvailable = queue.IsFundAvailable;
            }
            catch (Exception ex)
            {
                throw;
            }

            return MessageId;
        }

        private void CreateAdHocNotificationList(int tblmessageid, int MappingID, int SourceObjectID, int CompanyId, int CurrentUserId, string TimeZoneId)
        {
            AdhocMessageNotificationList tblAdHocNotiLst = new AdhocMessageNotificationList()
            {
                CompanyId = CompanyId,
                MessageId = tblmessageid,
                ObjectMappingId = MappingID,
                SourceObjectPrimaryId = SourceObjectID,
                Status = 1,
                CreatedBy = CurrentUserId,
                CreatedOn = DateTime.Now,
                UpdatedBy = CurrentUserId,
                UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
            };
            _context.Set<AdhocMessageNotificationList>().Add(tblAdHocNotiLst);
            _context.SaveChanges();
        }

        public dynamic ProcessPAFile(string userListFile, bool hasHeader, int emailColIndex, int phoneColIndex, int postcodeColIndex, int latColIndex, int longColIndex, string sessionId)
        {

            try
            {

                string AttachmentSavePath = _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
                string AttachmentUncUser = _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
                string AttachmentUncPwd = _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
                string AttachmentUseUnc = _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
                string ServerUploadFolder = _DBC.LookupWithKey("UPLOAD_PATH");

                try
                {

                    _context.Database.ExecuteSqlRaw("DELETE FROM PublicAlertMessageListDump WHERE SessionId='" + sessionId + "'");

                    _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);

                    if (File.Exists(ServerUploadFolder + userListFile))
                    {
                        bool HeaderSkipped = false;
                        List<PublicAlertUserList> PauList = new List<PublicAlertUserList>();
                        DateTimeOffset dtnow = _DBC.GetDateTimeOffset(DateTime.Now);

                        using (var stream = File.Open(ServerUploadFolder + userListFile, FileMode.Open, FileAccess.Read))
                        {

                            using (var reader = GetExcelReaderFactory(ServerUploadFolder + userListFile, stream))
                            {
                                do
                                {
                                    while (reader.Read())
                                    {
                                        if (hasHeader && HeaderSkipped == false)
                                        {
                                            HeaderSkipped = true;
                                            continue;
                                        }
                                        else
                                        {
                                            var pau = new PublicAlertUserList();
                                            if (emailColIndex >= 0)
                                            {
                                                var emailval = reader.GetValue(emailColIndex) ?? "";
                                                pau.EmailId = Convert.ToString(emailval);
                                            }
                                            if (phoneColIndex >= 0)
                                            {
                                                var phoneval = reader.GetValue(phoneColIndex) ?? "";
                                                pau.MobileNo = Convert.ToString(phoneval);
                                            }
                                            if (postcodeColIndex >= 0)
                                            {
                                                var postcodeval = reader.GetValue(postcodeColIndex) ?? "";
                                                pau.Postcode = Convert.ToString(postcodeval);
                                            }
                                            if (latColIndex >= 0)
                                            {
                                                var latval = reader.GetValue(latColIndex) ?? "";
                                                pau.Latitude = Convert.ToString(latval);
                                            }
                                            if (longColIndex >= 0)
                                            {
                                                var longval = reader.GetValue(longColIndex) ?? "";
                                                pau.Longitude = Convert.ToString(longval);
                                            }
                                            PauList.Add(pau);
                                        }
                                    }
                                } while (reader.NextResult());
                            }
                        }

                        if (PauList.Count > 0)
                        {
                            //add data to datatable
                            DataTable dt = new DataTable();
                            dt.Columns.Add("EmailID", typeof(string));
                            dt.Columns.Add("MobileNo", typeof(string));
                            dt.Columns.Add("Postcode", typeof(string));
                            dt.Columns.Add("Latitude", typeof(string));
                            dt.Columns.Add("Longitude", typeof(string));
                            dt.Columns.Add("SessionId", typeof(string));
                            dt.Columns.Add("PublicAlertID", typeof(int));
                            dt.Columns.Add("CreatedOn", typeof(DateTimeOffset));

                            string SessionId = Guid.NewGuid().ToString();

                            foreach (PublicAlertUserList pauser in PauList)
                            {
                                dt.Rows.Add(new object[] { pauser.EmailId, pauser.MobileNo, pauser.Postcode, pauser.Latitude, pauser.Longitude, SessionId, 0, dtnow });
                            }

                            string constr = _context.Database.GetConnectionString()!;
                            var con = new SqlConnection(constr);
                            con.Open();
                            SqlBulkCopy objbulk = new SqlBulkCopy(con);

                            objbulk.DestinationTableName = "PublicAlertMessageListDump";
                            //Mapping Table column  
                            objbulk.ColumnMappings.Add("EmailID", "EmailID");
                            objbulk.ColumnMappings.Add("MobileNo", "MobileNo");
                            objbulk.ColumnMappings.Add("Postcode", "Postcode");
                            objbulk.ColumnMappings.Add("Latitude", "Latitude");
                            objbulk.ColumnMappings.Add("Longitude", "Longitude");
                            objbulk.ColumnMappings.Add("SessionId", "SessionId");
                            objbulk.ColumnMappings.Add("PublicAlertID", "PublicAlertID");
                            objbulk.ColumnMappings.Add("CreatedOn", "CreatedOn");

                            //inserting bulk Records into DataBase
                            objbulk.WriteToServer(dt);
                            con.Close();
                            con.Dispose();
                            dt.Dispose();

                            var rows = PauList.Take(200).ToList();
                            var emailcount = PauList.Where(w => w.EmailId != null && w.EmailId != "").ToList().Count;
                            var phonecount = PauList.Where(w => w.MobileNo != null && w.MobileNo != "").ToList().Count;
                            return new { rows, emailcount, phonecount, total = phonecount, SessionId };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        private IExcelDataReader GetExcelReaderFactory(string path, FileStream stream)
        {
            if (Path.GetExtension(path) == ".csv")
            {
                return ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration() { AutodetectSeparators = new char[] { ',', ';', '\t' } });
            }
            else
            {
                return ExcelReaderFactory.CreateReader(stream);
            }
        }

        public async Task<dynamic> ResendFailure(int messageId, string commsMethod)
        {
            CommonDTO ResultDTO = new CommonDTO();
            try
            {
                var deviceList = _queueHelper.GetFailedDeviceQueue(messageId, commsMethod, 0);
                if (deviceList.Count > 0)
                {
                    bool queued = await _queueHelper.RequeueMessage(messageId, commsMethod, deviceList);
                    if (queued)
                    {
                        ResultDTO.ErrorId = 205;
                        ResultDTO.ErrorCode = "205";
                        ResultDTO.Message = "Message queued successfully";
                    }
                }
                else
                {
                    ResultDTO.ErrorId = 206;
                    ResultDTO.ErrorCode = "206";
                    ResultDTO.Message = "No items found for resending";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }

        }

        public async Task<dynamic> SendPublicAlert(string messageText, int[] messageMethod, bool schedulePA, DateTime scheduleAt, string sessionId, int userId, int companyId, string timeZoneId)
        {
            try
            {

                try
                {

                    DateTimeOffset dtnow = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                    var PACount = (from PAD in _context.Set<PublicAlertMessageListDump>() where PAD.SessionId == sessionId select PAD).Count();

                    if (PACount > 0)
                    {
                        _MSG.MessageSourceAction = SourceAction.PublicAlert;
                        //Create message enry
                        int tblmessageid = await _MSG.CreateMessage(companyId, messageText, "PublicAlert", 0, 100, userId,
                                1, _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId), false, null, 99, 0, 0, false, false, messageMethod, null, 0);

                        //Create public alert entry for the listing
                        int PublicAlertID = await CreatePublicAlert(tblmessageid, schedulePA, scheduleAt, userId, timeZoneId);

                        int PhoneAdded = 0;
                        int TextAdded = 0;
                        int EmailAdded = 0;

                        if (messageMethod.Contains(3))
                            PhoneAdded = 1;
                        if (messageMethod.Contains(2))
                            TextAdded = 1;
                        if (messageMethod.Contains(4))
                            EmailAdded = 1;


                        if (!schedulePA)
                        {
                            //Create Message List for Ping
                            _MSG.SavePublicAlertMessageList(sessionId, PublicAlertID, tblmessageid, dtnow, TextAdded, EmailAdded, PhoneAdded);

                            var rabbithosts = _queueHelper.RabbitHosts();
                            await Task.Factory.StartNew(() => _queueHelper.PublishPublicAlertQueue(tblmessageid, rabbithosts, "EMAIL")).ContinueWith(t => {
                                t.Dispose();
                            });
                           await Task.Factory.StartNew(() => _queueHelper.PublishPublicAlertQueue(tblmessageid, rabbithosts, "TEXT")).ContinueWith(t => {
                                t.Dispose();
                            });
                            _context.Database.ExecuteSqlRaw("EXEC UPDATE PublicAlert SET Executed=1 WHERE PublicAlertID=" + PublicAlertID);
                        }
                        else
                        {
                            _context.Database.ExecuteSqlRaw("EXEC UPDATE PublicAlertMessageListDump SET PublicAlertID=" + PublicAlertID + " WHERE SessionId='" + sessionId + "'");

                            //SchedulerHelper SH = new SchedulerHelper();
                            //DateTimeOffset DOScheduleAt = _DBC.GetDateTimeOffset(ScheduleAt, TimeZoneId);
                            //bool CanRun = SH.ScheudlePublicAlert(tblmessageid, PublicAlertID, SessionId, DOScheduleAt, TimeZoneId);
                            //if (!CanRun)
                            //{
                            //    CommonDTO DT = new CommonDTO();
                            //    DT.ErrorCode = "220";
                            //    DT.Message = "Cannot schedule this public alert";
                            //    return DT;
                            //}
                        }

                        return true;
                    }
                    // }
                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<int> CreatePublicAlert(int messageId, bool scheduled, DateTimeOffset scheduleAt, int userId, string timeZoneId)
        {
            try
            {
                PublicAlert PA = new PublicAlert()
                {
                    MessageId = messageId,
                    Scheduled = scheduled,
                    ScheduleAt = scheduleAt,
                    Executed = 0,
                    CreatedBy = userId,
                    CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId)
                };
                _context.Set<PublicAlert>().Add(PA);
                await _context.SaveChangesAsync();
                return PA.PublicAlertId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<dynamic> ReplyToMessage(int parentId, string messageText, string replyTo, string messageType, int activeIncidentId, int[] messageMethod,
            int cascadePlanId, int currentUserId, int companyId, string timeZoneId)
        {
            Return Result = new Return();
            try
            {
                var msg = (from M in _context.Set<Message>() where M.MessageId == parentId select M).FirstOrDefault();
                if (msg != null)
                {

                    int MessageActionType = 0;
                    bool MultiResponse = false;

                    _MSG.TimeZoneId = timeZoneId;
                    _MSG.CascadePlanID = cascadePlanId;
                    _MSG.MessageSourceAction = SourceAction.PingReply;

                    if (replyTo.ToUpper() == "RENOTIFY")
                    {
                        string NotifyNote = _DBC.GetCompanyParameter("RENOTIFY_NOTE", companyId);
                        messageText = msg.MessageText + Environment.NewLine + "[" + NotifyNote + "]";
                        MessageActionType = 1;
                        MultiResponse = msg.MultiResponse;
                    }

                    int tblmessageid = await _MSG.CreateMessage(companyId, messageText, messageType, activeIncidentId, 100, currentUserId,
                        1, _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId), MultiResponse, null, 99, msg.AssetId, 0, false, msg.SilentMessage, messageMethod, null,
                        parentId, MessageActionType);

                    msg.HasReply += 1;

                    if (msg.AttachmentCount > 0)
                    {
                        var newmsg = (from M in _context.Set<Message>() where M.MessageId == tblmessageid select M).FirstOrDefault();
                        if (newmsg != null)
                        {
                            newmsg.AttachmentCount = msg.AttachmentCount;
                        }
                    }
                    _context.SaveChanges();

                    int queuecount = await queue.CreateMessageList(tblmessageid, replyTo);
                    IsFundAvailable = queue.IsFundAvailable;

                    Result = _DBC.Return(0, IsFundAvailable == true ? null : "E219", true, "SUCCESS", tblmessageid, queuecount);

                    return Result;
                }
                else
                {
                    Result = _DBC.Return(214, "E214", false, "The source message is not found.", null);
                    return Result;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
