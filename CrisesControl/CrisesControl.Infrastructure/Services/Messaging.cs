using Azure.Storage.Files.Shares;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Reports.SP_Response;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MessageMethod = CrisesControl.Core.Messages.MessageMethod;

namespace CrisesControl.Infrastructure.Services
{
    public class Messaging
    {
        private readonly CrisesControlContext db;
        public bool TextUsed = false;
        public bool PhoneUsed = false;
        public bool EmailUsed = false;
        public bool PushUsed = false;
        public string TimeZoneId = "GMT Standard Time";
        public int CascadePlanID = 0;
        public string MessageSourceAction = "";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBCommon _DBC;
        //private readonly Messaging _MSG;
        private readonly SendEmail _SDE;
        public Messaging(CrisesControlContext _db, IHttpContextAccessor httpContextAccessor, DBCommon DBC)
        {
            db = _db;
            _httpContextAccessor = httpContextAccessor;
            _DBC = new DBCommon(db,_httpContextAccessor);
            //_MSG = new Messaging(db,_httpContextAccessor,DBC);
            _SDE = new SendEmail(db,DBC);
        }
 

        public void AddUserToNotify(int messageId, List<int> userId, int activeIncidentId = 0)
        {
            try
            {
                var ins = from I in userId
                          select new UsersToNotify
                          {
                              MessageId = messageId,
                              UserId = I,
                              ActiveIncidentId = activeIncidentId
                          };

                db.AddRange(ins);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateMessage(int companyId, string msgText, string messageType, int incidentActivationId, int priority, int currentUserId,
           int source, DateTimeOffset localTime, bool multiResponse, List<AckOption> ackOptions, int status = 0, int assetId = 0, int activeIncidentTaskId = 0,
           bool trackUser = false, bool silentMessage = false, int[] messageMethod = null, List<MediaAttachment> mediaAttachments = null, int parentId = 0,
           int messageActionType = 0)
        {
            try
            {

                if (parentId > 0 && incidentActivationId == 0 && messageType.ToUpper() == "INCIDENT")
                {
                    var parentmsg =await db.Set<Message>().Where(M=> M.MessageId == parentId).FirstOrDefaultAsync();
                    if (parentmsg != null)
                    {
                        incidentActivationId = parentmsg.IncidentActivationId;
                    }
                }

                Message tblMessage = new Message()
                {
                    CompanyId = companyId,
                    MessageText = !string.IsNullOrEmpty(msgText) ? msgText.Trim() : "",
                    MessageType = messageType,
                    IncidentActivationId = incidentActivationId,
                    Priority = priority,
                    Status = status,
                    CreatedBy = currentUserId,
                    CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    UpdatedBy = currentUserId,
                    UpdatedOn = localTime,
                    Source = source,
                    MultiResponse = multiResponse,
                    AssetId = assetId,
                    CreatedTimeZone = localTime,
                    ActiveIncidentTaskId = activeIncidentTaskId,
                    TrackUser = trackUser,
                    SilentMessage = silentMessage,
                    AttachmentCount = 0,
                    ParentId = parentId,
                    MessageActionType = messageActionType,
                    CascadePlanId = CascadePlanID,
                    MessageSourceAction = MessageSourceAction
                };
                await db.AddAsync(tblMessage);
                await db.SaveChangesAsync();

                if (multiResponse)
                    await SaveActiveMessageResponse(tblMessage.MessageId, ackOptions, incidentActivationId);

                //Add TrackMe Users in Notification.
                if (incidentActivationId > 0)
                  await   AddTrackMeUsers(incidentActivationId, tblMessage.MessageId, companyId);


                if (messageMethod != null && CascadePlanID <= 0)
                {
                    if (messageMethod.Length > 0)
                    {
                        await ProcessMessageMethod(tblMessage.MessageId, messageMethod, incidentActivationId, trackUser);
                    }
                    else
                    {

                        var commsmethod = await  db.Set<CrisesControl.Core.Messages.MessageMethod>()
                                           .Where(MM=> MM.ActiveIncidentId == incidentActivationId).Select(MM=>MM.MethodId).Distinct().ToArrayAsync();
                        if (commsmethod != null)
                        {
                            await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                        }
                    }
                }
                else if (CascadePlanID > 0)
                {
                    var methods = await db.Set<PriorityInterval>()
                                   .Where(PI=> PI.CascadingPlanId == CascadePlanID)
                                   .Select(PI=> PI.Methods).ToListAsync();
                    if (methods != null)
                    {
                        string channels = GetCascadeChannels(methods);
                        var commsmethod = channels.Split(',').Select(int.Parse).ToArray();
                        if (commsmethod != null)
                        {
                            await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                        }
                    }
                }
                else
                {
                    var incmsgid = await  db.Set<MessageMethod>().Where(w => w.ActiveIncidentId == incidentActivationId).OrderBy(o => o.MessageId).FirstOrDefaultAsync();
                    int[] commsmethod = new int[] { };

                    if (source == 3)
                    {
                        string comms_method = _DBC.GetCompanyParameter("TASK_SYSTEM_COMMS_METHOD", companyId);
                        commsmethod = comms_method.Split(',').Select(int.Parse).ToList().ToArray();
                    }
                    else
                    {
                        commsmethod = (from MM in db.Set<MessageMethod>()
                                       where ((MM.ActiveIncidentId == incidentActivationId && incidentActivationId > 0 && parentId == 0 &&
                                       MM.MessageId == incmsgid.MessageId) || (MM.MessageId == parentId && parentId > 0))
                                       select MM.MethodId).Distinct().ToArray();
                    }

                    if (commsmethod != null)
                    {
                        await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                    } // in the else check if the parentid is greater than 0 and commesmethod is null, take the incidentactiviation id
                    //from message table and requery the methdos from the last message.
                }

                tblMessage.Text = TextUsed;
                tblMessage.Phone = PhoneUsed;
                tblMessage.Email = EmailUsed;
                tblMessage.Push = PushUsed;
                db.SaveChangesAsync();

                //Process message attachments
                if (mediaAttachments != null)
                   await ProcessMessageAttachmentsAsync(tblMessage.MessageId, mediaAttachments, companyId, currentUserId, TimeZoneId);

                //Convert Asset to Message Attachment
                if (assetId > 0)
                    await CreateMessageAttachment(tblMessage.MessageId, assetId, companyId, currentUserId, TimeZoneId);

               await _DBC.MessageProcessLog(tblMessage.MessageId, "MESSAGE_CREATED");

                return tblMessage.MessageId;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }

        public string GetCascadeChannels(List<string> channels)
        {
            var cas_channel = string.Join(",", channels).Split(',').Distinct().ToList();
            return string.Join(",", cas_channel);
        }
        public async Task<bool> CanSendMessage(int companyId)
        {
            try
            {
                var comp = await db.Set<Company>().Where(w => w.CompanyId == companyId).FirstOrDefaultAsync();
                if (!comp.OnTrial)
                {
                    return true;
                }
                else
                {
                    var msgs = (from MT in db.Set<MessageTransaction>()
                                join M in db.Set<Message>() on MT.MessageId equals M.MessageId
                                where M.CompanyId == companyId
                                select MT).ToList();
                    if (msgs.Count > 5)
                    {

                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ProcessMessageAttachmentsAsync(int messageId, List<MediaAttachment> mediaAttachments, int companyId, int createdUserId, string timeZoneId)
        {
            try
            {
                string AttachmentSavePath = _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
                string AttachmentUncUser = _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
                string AttachmentUncPwd = _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
                string AttachmentUseUnc = _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
                string ServerUploadFolder = _DBC.LookupWithKey("UPLOAD_PATH");

                string hostingEnv = _DBC.Getconfig("HostingEnvironment");
                string apiShare = _DBC.Getconfig("AzureAPIShare");

                int Count = 0;
                if (mediaAttachments != null)
                {
                    try
                    {
                        if (hostingEnv.ToUpper() != "AZURE")
                        {
                            _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);
                        }

                        FileHandler FH = new FileHandler(db, _httpContextAccessor);
                        foreach (MediaAttachment ma in mediaAttachments)
                        {

                            try
                            {
                                if (File.Exists(@ServerUploadFolder + ma.FileName))
                                {

                                    string DirectoryPath = AttachmentSavePath + companyId.ToString() + "\\" + ma.AttachmentType.ToString() + "\\";
                                    if (hostingEnv.ToUpper() == "AZURE")
                                    {
                                        using (FileStream filestream = File.OpenRead(@ServerUploadFolder + ma.FileName))
                                        {
                                            var result = FH.UploadToAzure(apiShare, DirectoryPath, ma.FileName, filestream).Result;
                                        }
                                    }
                                    else
                                    {
                                        if (!Directory.Exists(DirectoryPath))
                                        {
                                            Directory.CreateDirectory(DirectoryPath);
                                        }
                                        if (File.Exists(@DirectoryPath + ma.FileName))
                                        {
                                            File.Delete(@DirectoryPath + ma.FileName);
                                        }
                                        File.Move(@ServerUploadFolder + ma.FileName, @DirectoryPath + ma.FileName);
                                    }
                                }
                                await CreateMediaAttachment(messageId, ma.Title, ma.FileName, ma.OriginalFileName, ma.FileSize, ma.AttachmentType, 0, createdUserId, TimeZoneId);
                                Count++;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                        if (Count > 0)
                        {
                            var msg = await db.Set<Message>().Where(M=> M.MessageId == messageId).FirstOrDefaultAsync();
                            if (msg != null)
                            {
                                msg.AttachmentCount = Count;
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateMessageAttachment(int messageId, int assetId, int companyId, int createdUserId, string timeZoneId)
        {
            try
            {
                string AttachmentSavePath = _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
                string AttachmentUncUser = _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
                string AttachmentUncPwd = _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
                string AttachmentUseUnc = _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
                string ServerUploadFolder = _DBC.LookupWithKey("UPLOAD_PATH");
                string AzureAPIShare = _DBC.Getconfig("AzureAPIShare");
                string AzurePortalShare = _DBC.Getconfig("AzurePortalShare");
                string HostingEnvironment = _DBC.Getconfig("HostingEnvironment");

                //string SaveStorage = DBC.LookupWithKey("ATTACHMENT_STORAGE");
                int Count = 0;

                try
                {
                    var asset =await db.Set<Assets>().Where(A=> A.AssetId == assetId).FirstOrDefaultAsync();
                    if (asset != null)
                    {
                        FileHandler FH = new FileHandler(db, _httpContextAccessor);
                        string portal = _DBC.LookupWithKey("PORTAL");
                        string file_url = "uploads/" + companyId + "/assets/" + asset.AssetType.ToLower() + "/" + asset.AssetPath;
                        string DirectoryPath = AttachmentSavePath + companyId.ToString() + "\\2\\";

                        ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                        if (HostingEnvironment.ToUpper() == "AZURE")
                        {
                            string connectionString = _DBC.Getconfig("AzureFileShareConnection");

                            ShareClient desti_share = new ShareClient(connectionString, AzureAPIShare);
                            ShareDirectoryClient desti_directory = desti_share.GetDirectoryClient(DirectoryPath.Replace("\\", "/"));
                            if (!desti_directory.Exists())
                            {
                                desti_directory.Create();
                            }
                            var result = FH.CopyFileAsync(AzurePortalShare, file_url, AzureAPIShare, @DirectoryPath + asset.AssetPath);

                            await CreateMediaAttachment(messageId, asset.AssetTitle, asset.AssetPath, asset.SourceFileName, (decimal)asset.AssetSize, 2, 0, createdUserId, timeZoneId);
                            Count++;
                        }
                        else
                        {
                            _DBC.connectUNCPath();
                            _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);

                            using (var client = new WebClient())
                            {
                                file_url = portal + file_url;
                                client.DownloadFile(file_url, ServerUploadFolder + asset.AssetPath);

                                if (File.Exists(ServerUploadFolder + asset.AssetPath))
                                {
                                    try
                                    {

                                        if (!Directory.Exists(DirectoryPath))
                                        {
                                            Directory.CreateDirectory(DirectoryPath);
                                        }
                                        if (File.Exists(@DirectoryPath + asset.AssetPath))
                                        {
                                            File.Delete(@DirectoryPath + asset.AssetPath);
                                        }
                                        File.Move(@ServerUploadFolder + asset.AssetPath, @DirectoryPath + asset.AssetPath);
                                        await CreateMediaAttachment(messageId, asset.AssetTitle, asset.AssetPath, asset.SourceFileName, (decimal)asset.AssetSize, 2, 0, createdUserId, TimeZoneId);
                                        Count++;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }
                        }
                    }

                    if (Count > 0)
                    {
                        var msg = await  db.Set<Message>().Where(M=> M.MessageId == messageId ).FirstOrDefaultAsync();
                        if (msg != null)
                        {
                            msg.AttachmentCount += Count;
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MessageDetails>> GetReplies(int parentId, int companyId, int userId, string source = "WEB")
        {
            try
            {
               

                    var pParentID = new SqlParameter("@ParentID", parentId);
                    var pCompanyID = new SqlParameter("@CompanyID", companyId);
                    var pUserID = new SqlParameter("@UserID", userId);
                    var pSource = new SqlParameter("@Source", source);

                var result = await db.Set<MessageDetails>().FromSqlRaw("exec Pro_User_Message_Reply @ParentID, @CompanyID, @UserID, @Source",
                    pParentID, pCompanyID, pUserID, pSource).ToListAsync();


                        result.Select(async c => {
                            c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                            c.AckOptions = await (from AK in db.Set<ActiveMessageResponse>()
                                            where AK.MessageId == c.MessageId
                                            orderby AK.ResponseCode
                                            select new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToListAsync();
                            return c;
                        }).ToList();

                    return result;
                
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }

        public async Task<List<AudioAssetReturn>> GetMessageAudio(int assetTypeId, int companyId, int userId, string Source = "APP")
        {
            try
            {
                

                    var pAssetTypeID = new SqlParameter("@AssetTypeID", assetTypeId);
                    var pCompanyID = new SqlParameter("@CompanyID", companyId);
                    var pUserID = new SqlParameter("@UserID", userId);
                    var pSource = new SqlParameter("@Source", Source);

                    var result = await db.Set<AudioAssetReturn>().FromSqlRaw("exec Pro_Get_Message_Audio @AssetTypeID, @CompanyID, @UserID, @Source",
                        pAssetTypeID, pCompanyID, pUserID, pSource).ToListAsync();

                    return result;
                
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }

        public async Task CreateIncidentNotificationList(int messageId, int incidentActivationId, int mappingId, int sourceId, int currentUserId, int companyId, string timeZoneId)
        {
            IncidentNotificationList tblIncidentNotiLst = new IncidentNotificationList()
            {
                CompanyId = companyId,
                IncidentActivationId = incidentActivationId,
                ObjectMappingId = mappingId,
                SourceObjectPrimaryId = sourceId,
                MessageId = messageId,
                Status = 1,
                CreatedBy = currentUserId,
                CreatedOn = DateTime.Now,
                UpdatedBy = currentUserId,
                UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
            };
           await  db.AddAsync(tblIncidentNotiLst);
            await db.SaveChangesAsync();
        }

        public async Task<int> CreateMediaAttachment(int messageId, string title, string filePath, string originalFileName, decimal fileSize, int attachmentType,
            int messageListId, int createdBy, string timeZoneId)
        {
            try
            {
                MessageAttachment MA = new MessageAttachment
                {
                    AttachmentType = attachmentType,
                    CreatedBy = createdBy,
                    CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    FilePath = filePath,
                    FileSize = fileSize,
                    MessageId = messageId,
                    MessageListId = messageListId,
                    OriginalFileName = originalFileName,
                    Title = title
                };
               await  db.AddAsync(MA);
                await db.SaveChangesAsync();
                return MA.MessageAttachmentId;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }

        }

        public async Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentactivationId, bool trackUser = false)
        {
            bool pushadded = false;
            try
            {
                int pushmethodid = 1;

                var methodlist = await db.Set<CommsMethod>().ToListAsync();

                if (trackUser)
                {
                    pushmethodid = methodlist.Where(w => w.MethodCode == "PUSH").Select(s => s.CommsMethodId).FirstOrDefault();
                }

                foreach (int Method in messageMethod)
                {
                    await CreateMessageMethod(messageId, Method, incidentactivationId);

                    string chkmethod = methodlist.Where(w => w.CommsMethodId == Method).Select(s => s.MethodCode).FirstOrDefault();
                    if (chkmethod == "TEXT")
                        TextUsed = true;

                    //var phnmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "PHONE").Select(s => s.MethodCode).Any();
                    if (chkmethod == "PHONE")
                        PhoneUsed = true;

                    //var emailmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "EMAIL").Select(s => s.MethodCode).Any();
                    if (chkmethod == "EMAIL")
                        EmailUsed = true;

                    //var pushmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "PUSH").Select(s => s.MethodCode).Any();
                    if (chkmethod == "PUSH")
                        PushUsed = true;

                    if (pushmethodid == Method)
                        pushadded = true;
                }
                if (trackUser && !pushadded)
                {
                   await  CreateMessageMethod(messageId, pushmethodid, incidentactivationId);
                    PushUsed = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0)
        {
            try
            {
                var exist = await  db.Set<MessageMethod>()
                             .Where(MMS=>
                                    MMS.ActiveIncidentId == activeIncidentId &&
                                    activeIncidentId > 0 &&
                                    MMS.MethodId == methodId).AnyAsync();

                MessageMethod MM = new MessageMethod()
                {
                    MessageId = messageId,
                    MethodId = methodId,
                    ActiveIncidentId = (exist == false ? activeIncidentId : 0),
                    IncidentId = incidentId
                };
                await db.AddAsync(MM);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0)
        {
            try
            {
                if (activeIncidentId == 0 && messageId > 0)
                {
                    var mt_list = await db.Set<MessageMethod>().Where(MM=> MM.MessageId == messageId).ToListAsync();
                    db.RemoveRange(mt_list);
                }
                else
                {
                    var mt_list = await db.Set<MessageMethod>().Where(MM => MM.ActiveIncidentId == activeIncidentId).ToListAsync();
                    db.RemoveRange(mt_list);
                }
               await  db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task SaveActiveMessageResponse(int messageId, List<AckOption> ackOptions, int incidentActivationId = 0)
        {
            try
            {

                //Deleting temp records
                var delete_old = await db.Set<ActiveMessageResponse>()
                                  .Where(AMR=> AMR.MessageId == 0 && AMR.ActiveIncidentId == incidentActivationId
                                ).ToListAsync();
                db.RemoveRange(delete_old);
                await db.SaveChangesAsync();

                foreach (AckOption ao in ackOptions)
                {
                    var option = await  db.Set<CompanyMessageResponse>().Where(w => w.ResponseId == ao.ResponseId).FirstOrDefaultAsync();
                    if (option != null)
                    {
                        ActiveMessageResponse AC = new ActiveMessageResponse
                        {
                            MessageId = messageId,
                            ResponseId = ao.ResponseId,
                            ResponseCode = ao.ResponseCode,
                            ResponseLabel = option.ResponseLabel,
                            IsSafetyResponse = option.IsSafetyResponse,
                            SafetyAckAction = option.SafetyAckAction,
                            ActiveIncidentId = incidentActivationId
                        };
                       await db.AddAsync(AC);
                    }
                }
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateProcessQueue(int messageId, string messageType, string method, string state, int priority)
        {
            try
            {
                var queue = await db.Set<ProcessQueue>()
                             .Where(PQ=> PQ.Method == method && PQ.MessageId == messageId).FirstOrDefaultAsync();
                if (queue == null)
                {
                    ProcessQueue INPQ = new ProcessQueue
                    {
                        MessageId = messageId,
                        CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now),
                        Priority = priority,
                        MessageType = messageType,
                        Method = method,
                        State = state
                    };
                   await  db.AddAsync(INPQ);
                   await  db.SaveChangesAsync();
                }
                else
                {
                    if (queue != null)
                    {
                        queue.State = state;
                        await db.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NotificationDetails>> MessageNotifications(int companyId, int currentUserId)
        {
            List<NotificationDetails> NDL = new List<NotificationDetails>();

            List<IIncidentMessages> IM =await _get_incident_message(currentUserId, companyId);
            List<IPingMessage> PM = await _get_ping_message(currentUserId, companyId);

            NDL.Add(new NotificationDetails { IncidentMessages = IM, PingMessage = PM });
            return NDL;
        }

        public async Task<UserMessageCountModel> MessageNotificationsCount(int UserID)
        {
            try
            {
                var pUserID = new SqlParameter("@UserID", UserID);
               
                    var result = await  db.Set<UserMessageCountModel>().FromSqlRaw(" exec User_Get_Message_Count @UserID", pUserID).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        return new UserMessageCountModel();
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new UserMessageCountModel();
        }

        public async Task<List<IIncidentMessages>> _get_incident_message(int userId, int companyId)
        {
            try
            {
                

                    var pTargetUserId = new SqlParameter("@UserID", userId);
                    var pCompanyID = new SqlParameter("@CompanyID", companyId);

                var result = await db.Set<IIncidentMessages>().FromSqlRaw("exec Pro_User_Incident_Notifications @UserID, @CompanyID",
                    pTargetUserId, pCompanyID).ToListAsync();

                result.Select(async c => {
                            c.AckOptions = await db.Set<ActiveMessageResponse>()
                                            .Where(AK=> AK.MessageId == c.MessageId && c.MultiResponse == true)
                                            .OrderBy(AK=> AK.ResponseCode)
                                            .Select(AK=> new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToListAsync();
                            return c;
                        }).ToList();
                    return result;
               
            }
            catch (Exception ex)
            {
                throw ex;
                return new List<IIncidentMessages>();
            }
        }

        public async Task<List<IPingMessage>> _get_ping_message(int userId, int companyId)
        {
            try
            {
                

                    var pTargetUserId = new SqlParameter("@UserID", userId);
                    var pCompanyID = new SqlParameter("@CompanyID", companyId);

                var result = await db.Set<IPingMessage>().FromSqlRaw("exec Pro_User_Ping_Notifications @UserID, @CompanyID",
                    pTargetUserId, pCompanyID).ToListAsync();


                        result.Select(c => {
                            c.AckOptions = db.Set<ActiveMessageResponse>()
                                            .Where(AK=> AK.MessageId == c.MessageId && c.MultiResponse == true)
                                            .OrderBy(AK=> AK.ResponseCode)
                                            .Select(AK=> new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToList();
                            return c;
                        }).ToList();
                    return result;
                
            }
            catch (Exception ex)
            {
                throw ex;
                return new List<IPingMessage>();
            }
        }

        public async Task<bool> StartConference(List<User> userList, int objectId, int currentUserId, int companyId, string timeZoneId)
        {
        
            try
            {
                string ConServiceEnable = _DBC.GetCompanyParameter("CONCIERGE_SERVICE", companyId);

                var phone_method = (from CM in db.Set<CompanyComm>()
                                    join CO in db.Set<CommsMethod>() on CM.MethodId equals CO.CommsMethodId
                                    where CM.CompanyId == companyId && CO.MethodCode == "PHONE" && CM.Status == 1 && CM.ServiceStatus == true
                                    select CO).FirstOrDefault();

                if (ConServiceEnable == "true" && phone_method != null)
                {

                    if (userList.Count > 0)
                    {
                        
                        var result = StartConferenceNew(companyId, currentUserId, userList, timeZoneId, objectId, 0);
                        return true;

                    }
                    //else
                    //{
                    //    ResultDTO.ErrorId = 100;
                    //    ResultDTO.Message = "An error has occured";
                    //}
                }
                return false;
                //else
                //{
                //    ResultDTO.ErrorId = 200;
                //    ResultDTO.Message = "Conference service not enabled";
                //}
                //return ResultDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident")
        {
            
            //Initiatize the variables
            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string FromNumber = string.Empty;
            string APIClass = string.Empty;
            string CallBackUrl = string.Empty;
            string MessageXML = string.Empty;
            string TwilioRoutingApi = string.Empty;

            try
            {

                //Get the selected conferance api for the company and set the requrest api params.
                string CONF_API = _DBC.GetCompanyParameter("CONFERANCE_API", companyId);
                bool RecordConf = Convert.ToBoolean(_DBC.GetCompanyParameter("RECORD_CONFERENCE", companyId));
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                //Create instance of CommsApi choosen by company
                dynamic CommsAPI = _DBC.InitComms(CONF_API);

                CommsAPI.IsConf = true;
                CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                //Get API configraiton from sysparameters
                string RetryNumberList = _DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", companyId, FromNumber);
                List<string> FromNumberList = RetryNumberList.Split(',').ToList();

                FromNumber = FromNumberList.FirstOrDefault();
                CallBackUrl = _DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                MessageXML = _DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

                //Get the user list to fetch their mobile numbers
                List<int> nUList = new List<int>();
                nUList.Add(userId);

                foreach (User cUser in userList)
                {
                    nUList.Add(cUser.UserId);
                }

                var tmpUserList = await db.Set<User>()
                                   .Where(U=> U.CompanyId == companyId && nUList.Contains(U.UserId) && U.Status == 1)
                                   .Select(U=> new { UserId = U.UserId, ISD = U.Isdcode, PhoneNumber = U.MobileNo, U.Llisdcode, U.Landline }).Distinct().ToListAsync();


                //Create conference header
                int ConfHeaderId =await  CreateConferenceHeader(companyId, userId, TimeZoneId, CommsAPI.ConfRoom, RecordConf, activeIncidentId, messageId, objectType);

                string CallId = string.Empty;
                int ModeratorCDId = 0;
                string ModeratorNumber = string.Empty;
                string ModeratorLandline = string.Empty;
                string Status = string.Empty;

                //Loop through with each user and their phone number to make the call
                foreach (var uItem in tmpUserList)
                {
                    string Mobile = _DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);
                    string Landline = _DBC.FormatMobile(uItem.Llisdcode, uItem.Landline);

                    if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                    {
                        int CallDetaildId =await CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, Mobile, Landline, CallId.ToString(), "ADDED", TimeZoneId, 0);
                        if (uItem.UserId == userId)
                        {
                            ModeratorCDId = CallDetaildId;
                            ModeratorNumber = Mobile;

                            if (!string.IsNullOrEmpty(Landline))
                                ModeratorLandline = Landline;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ModeratorNumber))
                {
                    string CallStatus = string.Empty;
                    string CalledOn = "MOBILE";

                    CallStatus = MakeConferenceCall(FromNumber, CallBackUrl, MessageXML, CommsAPI, out CallId, ModeratorNumber, ModeratorLandline, out Status, CalledOn);
                    await CreateConferenceDetail("UPDATE", 0, 0, "", "", CallId.ToString(), CallStatus, "", ModeratorCDId, CalledOn);
                }
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public string MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI, out string callId,
            string mobileNumber, string landLineNumber, out string status, string calledOn)
        {
            calledOn = "MOBILE";
            string CallStatus = string.Empty;
            //Initiate the call to the modrator
            Task<dynamic> calltask = Task.Factory.StartNew(() => commsAPI.Call(fromNumber, mobileNumber, messageXML, callBackUrl));
            CommsStatus callrslt = calltask.Result;
            callId = callrslt.CommsId;

            if (!callrslt.Status)
            {
                status = "";
                CallStatus = callId = _DBC.Left(callId, 50);
                if (!string.IsNullOrEmpty(landLineNumber))
                {
                    Task<dynamic> recalltask = Task.Factory.StartNew(() => commsAPI.Call(fromNumber, landLineNumber, messageXML, callBackUrl));
                    CommsStatus recallrslt = recalltask.Result;
                    if (recallrslt.Status)
                    {
                        calledOn = "LANDLINE";
                        CallStatus = recallrslt.CurrentAction;
                        status = recallrslt.CurrentAction;
                    }
                    else
                    {
                        callId = recallrslt.CommsId;
                        status = "";
                        CallStatus = callId = _DBC.Left(callId, 50);
                    }
                }
            }
            else
            {
                status = callrslt.CurrentAction;
                CallStatus = callrslt.CurrentAction;
            }
            return CallStatus;
        }

        //public object GetConfRecordings(int ConfCallId, int ObjectID, string ObjectType, bool Single, int CompanyID)
        //{
        //    CommonDTO ResultDTO = new CommonDTO();
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        ObjectType = ObjectType.ToUpper();
        //        if (ConfCallId <= 0)
        //        {
        //            var recordings = (from CH in db.ConferenceCallLogHeader
        //                              join U in db.Users on CH.InitiatedBy equals U.UserId
        //                              where CH.CompanyId == CompanyID && CH.TargetObjectId == ObjectID && CH.TargetObjectName == ObjectType
        //                              select new
        //                              {
        //                                  CH,
        //                                  U.FirstName,
        //                                  U.LastName,
        //                                  ConferenceStart = CH.ConfrenceStart != null ? CH.ConfrenceStart : CH.CreatedOn,
        //                                  ConferenceEnd = CH.ConfrenceEnd != null ? CH.ConfrenceEnd : CH.CreatedOn
        //                              }).ToList();

        //            return recordings;
        //        }
        //        else if (ConfCallId > 0 && Single == true)
        //        {
        //            var recording = (from CH in db.ConferenceCallLogHeader
        //                             where CH.ConferenceCallId == ConfCallId
        //                             select CH).FirstOrDefault();
        //            if (recording != null)
        //            {
        //                if (recording.RecordingSid != null)
        //                {
        //                    DownloadRecording(recording.RecordingSid, recording.CompanyId, recording.RecordingURL);
        //                }
        //            }
        //            return recording;
        //        }
        //        else if (ConfCallId > 0 && Single == false)
        //        {
        //            var recordings = (from CD in db.ConferenceCallLogDetail
        //                              join CH in db.ConferenceCallLogHeader on CD.ConferenceCallId equals CH.ConferenceCallId
        //                              join UD in db.Users on CD.UserId equals UD.UserId
        //                              where CD.ConferenceCallId == ConfCallId
        //                              orderby CD.UserId
        //                              select new
        //                              {
        //                                  CD,
        //                                  CD.UserId,
        //                                  UD.FirstName,
        //                                  UD.LastName,
        //                                  UD.ISDCode,
        //                                  UD.MobileNo,
        //                                  CH.ConfrenceEnd,
        //                                  CH.ConfrenceStart
        //                              }).ToList().OrderBy(o => o.CD.UserId);

        //            return recordings;
        //        }
        //        return ResultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        ResultDTO = DBC.catchException(ex);
        //        return ResultDTO;
        //    }
        //}

        //public void DownloadRecording(string RecordingSid, int CompanyId, string RecordingUrl)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        int RetryCount = 2;
        //        string ServerUploadFolder = HttpContext.Current.Server.MapPath("../../tmp/");
        //        string hostingEnv = DBC.Getconfig("HostingEnvironment");
        //        string AzureAPIShare = DBC.Getconfig("AzureAPIShare");

        //        string RecordingPath = DBC.LookupWithKey("RECORDINGS_PATH");
        //        int.TryParse(DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT"), out RetryCount);
        //        string SavePath = @RecordingPath + "\\" + CompanyId + "\\";

        //        DBC.connectUNCPath();

        //        FileHandler FH = new FileHandler();

        //        if (FH.FileExists(RecordingSid + ".mp3", AzureAPIShare, SavePath))
        //        {
        //            return;
        //        }

        //        if (hostingEnv != "AZURE")
        //        {
        //            if (!Directory.Exists(SavePath))
        //                Directory.CreateDirectory(SavePath);
        //        }

        //        try
        //        {
        //            WebClient Client = new WebClient();
        //            bool confdownloaded = false;
        //            bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
        //            string RoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

        //            for (int i = 0; i < RetryCount; i++)
        //            {
        //                try
        //                {
        //                    if (SendInDirect)
        //                    {
        //                        RecordingUrl = RoutingApi + "Communication/DownloadRecording?FileName=" + RecordingSid;
        //                    }
        //                    else
        //                    {
        //                        RecordingUrl += ".mp3";
        //                    }
        //                    Client.DownloadFile(RecordingUrl, @ServerUploadFolder + RecordingSid + ".mp3");
        //                    confdownloaded = true;
        //                    break;
        //                }
        //                catch (Exception ex)
        //                {
        //                    DBC.catchException(ex, "CommnicationHelper->HandConfRecording" + RecordingUrl, "Retry " + i);
        //                    confdownloaded = true;
        //                }
        //            }
        //            if (confdownloaded)
        //            {
        //                if (File.Exists(@ServerUploadFolder + RecordingSid + ".mp3"))
        //                {
        //                    using (FileStream filestream = File.OpenRead(@ServerUploadFolder + RecordingSid + ".mp3"))
        //                    {
        //                        if (hostingEnv == "AZURE")
        //                        {
        //                            var result = FH.UploadToAzure(AzureAPIShare, RecordingPath, RecordingSid + ".mp3", filestream).Result;
        //                        }
        //                        else
        //                        {
        //                            File.Copy(@ServerUploadFolder + RecordingSid + ".mp3", SavePath + RecordingSid + ".mp3");
        //                        }
        //                    }

        //                    CommsHelper CH = new CommsHelper();
        //                    CH.DeleteRecording(RecordingSid);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            DBC.catchException(ex);
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        DBC.catchException(ex, "Communication", "HandleConfRecording->Download Recording", 0);
        //    }
        //}

        //public object GetConfUser(int ObjectID, string ObjectType)
        //{
        //    DBCommon DBC = new DBCommon();
        //    CommonDTO ResultDTO = new CommonDTO();
        //    try
        //    {
        //        if (ObjectType.ToUpper() == "INCIDENT")
        //        {
        //            var rcpnt_list = (from ML in db.MessageList
        //                              join M in db.Message on ML.MessageId equals M.MessageId
        //                              join U in db.Users on ML.RecepientUserId equals U.UserId
        //                              where M.IncidentActivationId == ObjectID && U.Status == 1 && M.MessageType == "Incident"
        //                              select new { U.FirstName, U.LastName, U.UserPhoto, U.ISDCode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToList();

        //            return rcpnt_list;
        //        }
        //        else if (ObjectType.ToUpper() == "PING")
        //        {
        //            var rcpnt_list = (from ML in db.MessageList
        //                              join M in db.Message on ML.MessageId equals M.MessageId
        //                              join U in db.Users on ML.RecepientUserId equals U.UserId
        //                              where M.MessageId == ObjectID && U.Status == 1 && M.MessageType == "Ping"
        //                              select new { U.FirstName, U.LastName, U.UserPhoto, U.ISDCode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToList();
        //            return rcpnt_list;
        //        }
        //        return ResultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        ResultDTO = DBC.catchException(ex);
        //        return ResultDTO;
        //    }

        //}

        //public object GetMessageDetails(string CloudMsgId, int MessageId = 0)
        //{
        //    DBCommon DBC = new DBCommon(db,_httpContextAccessor);

        //    try
        //    {
        //        int MsgListid = Convert.ToInt32(DBC.Base64Decode(CloudMsgId));

        //        if (MessageId > 0)
        //        {
        //            var msglistid = (from ML in db.MessageList
        //                             where ML.MessageListId == MsgListid
        //                             select new
        //                             {
        //                                 ML.RecepientUserId
        //                             }).FirstOrDefault();
        //            if (msglistid != null)
        //            {
        //                var newmsglistid = (from ML in db.MessageList
        //                                    where ML.MessageId == MessageId && ML.RecepientUserId == msglistid.RecepientUserId && ML.MessageAckStatus == 0
        //                                    select new
        //                                    {
        //                                        ML.MessageListId
        //                                    }).FirstOrDefault();
        //                if (newmsglistid != null)
        //                {
        //                    MsgListid = newmsglistid.MessageListId;
        //                }

        //            }
        //        }

        //        var CheckType = (from ML in db.MessageList
        //                         join M in db.Message on ML.MessageId equals M.MessageId
        //                         where ML.MessageListId == MsgListid
        //                         select new
        //                         {
        //                             M.MessageId,
        //                             M.MessageType,
        //                             M.CompanyId,
        //                             ML.RecepientUserId,
        //                             M.IncidentActivationId
        //                         }).FirstOrDefault();

        //        if (CheckType != null)
        //        {
        //            if (CheckType.MessageType == "Incident")
        //            {
        //                var IncidentMessageDetails = (from IA in db.IncidentActivation
        //                                              join L in db.Location on IA.ImpactedLocationId equals L.LocationId
        //                                              join M in db.Message on IA.IncidentActivationId equals M.IncidentActivationId
        //                                              join ML in db.MessageList on M.MessageId equals ML.MessageId
        //                                              join U in db.Users on M.CreatedBy equals U.UserId
        //                                              join UR in db.Users on ML.RecepientUserId equals UR.UserId
        //                                              where ML.RecepientUserId == CheckType.RecepientUserId
        //                                              && IA.CompanyId == CheckType.CompanyId
        //                                              && IA.IncidentActivationId == CheckType.IncidentActivationId
        //                                              && M.MessageId == CheckType.MessageId
        //                                              select new
        //                                              {
        //                                                  ActiveIncidentID = IA.IncidentActivationId,
        //                                                  IncidentLaunchedDate = IA.LaunchedOn,
        //                                                  MessageListId = ML.MessageListId,
        //                                                  IncidentIcon = (IA.IncidentIcon == null || IA.IncidentIcon == "") ? "" : IA.IncidentIcon,
        //                                                  Severity = IA.Severity,
        //                                                  M.MessageText,
        //                                                  SentDateTime = ML.DateSent,
        //                                                  SentBy = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
        //                                                  Title = IA.Name,
        //                                                  ImpactedLocation = L.Location_Name,
        //                                                  LocLat = L.Lat,
        //                                                  LocLng = L.Long,
        //                                                  MultiResponse = M.MultiResponse,
        //                                                  ML.MessageAckStatus,
        //                                                  U.UserId,
        //                                                  U.Password,
        //                                                  CheckType.MessageType,
        //                                                  ML.AckMethod,
        //                                                  CheckType.CompanyId,
        //                                                  M.Source,
        //                                                  ML.IsTaskRecepient,
        //                                                  IA.HasNotes,
        //                                                  RecepientUserId = UR.UserId,
        //                                                  RecepientPassword = UR.Password,
        //                                                  M.AttachmentCount,
        //                                                  M.MessageId,
        //                                                  AckOptions = (from AK in db.ActiveMessageResponse
        //                                                                where AK.MessageID == CheckType.MessageId
        //                                                                orderby AK.ResponseCode
        //                                                                select new { AK.ResponseID, AK.ResponseLabel }).ToList()
        //                                              }).FirstOrDefault();
        //                return IncidentMessageDetails;
        //            }
        //            else if (CheckType.MessageType == "Ping")
        //            {
        //                var PingMessageDetails = (from M in db.Message
        //                                          join ML in db.MessageList on M.MessageId equals ML.MessageId
        //                                          join U in db.Users on M.CreatedBy equals U.UserId
        //                                          join UR in db.Users on ML.RecepientUserId equals UR.UserId
        //                                          orderby ML.MessageSentStatus ascending, ML.CreatedOn descending
        //                                          where ML.RecepientUserId == CheckType.RecepientUserId &&
        //                                          M.CompanyId == CheckType.CompanyId &&
        //                                          M.MessageType == "Ping" &&
        //                                          M.MessageId == CheckType.MessageId
        //                                          //&& ML.MessageSentStatus == 1
        //                                          select new
        //                                          {
        //                                              U.UserId,
        //                                              U.Password,
        //                                              CheckType.MessageType,
        //                                              MessageListId = ML.MessageListId,
        //                                              M.MessageText,
        //                                              MultiResponse = M.MultiResponse,
        //                                              SentDateTime = ML.DateSent,
        //                                              SentBy = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
        //                                              ML.MessageAckStatus,
        //                                              ML.AckMethod,
        //                                              CheckType.CompanyId,
        //                                              RecepientUserId = UR.UserId,
        //                                              RecepientPassword = UR.Password,
        //                                              M.AttachmentCount,
        //                                              M.MessageId,
        //                                              M.MessageActionType,
        //                                              AckOptions = (from AK in db.ActiveMessageResponse
        //                                                            where AK.MessageID == CheckType.MessageId
        //                                                            orderby AK.ResponseCode
        //                                                            select new { AK.ResponseID, AK.ResponseLabel }).ToList()
        //                                          }).FirstOrDefault();
        //                return PingMessageDetails;
        //            }

        //        }
        //        else
        //        {
        //            ResultDTO.ErrorId = 110;
        //            ResultDTO.Message = "No record found.";
        //        }
        //        return ResultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public object GetPublicAlert(int companyId, int currentUserId)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<List<MessageDetails>> GetMessages(int CompanyId, int IncidentActivationId, int TargetUserId, string MessageType, int CurrentUserId)
        //{
        //    DBCommon DBC = new DBCommon(db,_httpContextAccessor);
        //    try
        //    {

        //            var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
        //            var pTargetUserId = new SqlParameter("@UserID", TargetUserId);
        //            var pMessageType = new SqlParameter("@MessageType", MessageType);
        //            var pIncidentActivationId = new SqlParameter("@IncidentActivationId", IncidentActivationId);

        //        var result = await db.Set<MessageDetails>().FromSqlRaw("exec Pro_User_Ping @CompanyID, @UserID, @MessageType, @IncidentActivationId",
        //            pCompanyID, pTargetUserId, pMessageType, pIncidentActivationId).ToListAsync();
        //                result.Select(c => {
        //                    c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
        //                    return c;
        //                }).ToList();

        //            return result;

        //    }
        //    catch (Exception ex)
        //    {
        //      throw Exception;
        //        return new List<MessageDetails>();
        //    }
        //}

        //public MessageAckDetails MessageAcknowledged(int CompanyId, int MsgListId, string TimeZoneId, string UserLocationLat, string UserLocationLong, int CurrentUserId, int ResponseID = 0, string AckMethod = "WEB")
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        Messaging MSG = new Messaging();
        //        AcknowledgeReturn MessageData = MSG.AcknowledgeMessage(CurrentUserId, 0, MsgListId, UserLocationLat, UserLocationLong, AckMethod, ResponseID, TimeZoneId);

        //        if (MessageData != null)
        //        {

        //            if (ResponseID > 0)
        //            {
        //                int CallbackOption = GetCallbackOption(AckMethod);
        //                CheckSOSAlert(MsgListId, "ACKNOWLEGE", CallbackOption);
        //            }

        //            if (AckMethod != "APP")
        //            {
        //                DBC.AddUserTrackingDevices(CurrentUserId);
        //            }

        //            return (from MsgList in db.MessageList
        //                    where MsgList.MessageListId == MsgListId
        //                    select new MessageAckDetails
        //                    {
        //                        MessageListId = MsgList.MessageListId,
        //                        ErrorId = 0,
        //                        ErrorCode = "E117",
        //                        Message = "Acknowleged"
        //                    }).FirstOrDefault();
        //        }
        //        else
        //        {
        //            return (from MsgList in db.MessageList
        //                    where MsgList.MessageListId == MsgListId
        //                    select new MessageAckDetails
        //                    {
        //                        MessageListId = MsgList.MessageListId,
        //                        ErrorId = 100,
        //                        ErrorCode = "150",
        //                        Message = "Message aleady acknowledged"
        //                    }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //    }
        //    return new MessageAckDetails();
        //}

        //public void CheckSOSAlert(int MessageListID, string SOSType, int CallbackOption)
        //{
        //    try
        //    {
        //        DBCommon DBC = new DBCommon();
        //        var sos = (from ML in db.MessageList
        //                   join M in db.Message on ML.MessageId equals M.MessageId
        //                   join AMR in db.ActiveMessageResponse on ML.ResponseID equals AMR.ResponseID
        //                   where ML.MessageListId == MessageListID
        //                   select new { ML, M, AMR }).FirstOrDefault();

        //        if (sos != null)
        //        {
        //            bool IsSOSEnabled = false;
        //            bool.TryParse(DBC.GetCompanyParameter("SOS_ENABLED", sos.M.CompanyId), out IsSOSEnabled);

        //            if (sos.AMR.IsSafetyResponse && IsSOSEnabled)
        //            {
        //                var check = (from SA in db.SOSAlert
        //                             where
        //      SA.UserID == sos.ML.RecepientUserId &&
        //      (SA.ActiveIncidentID == sos.M.IncidentActivationId &&
        //      SA.ActiveIncidentID != 0 && sos.M.IncidentActivationId != 0)
        //                             select SA).FirstOrDefault();
        //                if (check != null)
        //                {
        //                    check.ActiveIncidentID = sos.M.IncidentActivationId;
        //                    check.AlertType = SOSType;
        //                    check.Latitude = DBC.Left(sos.ML.UserLocationLat, 15);
        //                    check.Longitude = DBC.Left(sos.ML.UserLocationLong, 15);
        //                    check.MessageID = sos.M.MessageId;
        //                    check.MessageListID = sos.ML.MessageListId;
        //                    check.ResponseID = sos.ML.ResponseID;
        //                    check.ResponseLabel = sos.AMR.ResponseLabel;
        //                    check.ResponseTime = sos.ML.UpdatedOn;
        //                    check.ResponseTimeGMT = DBC.GetDateTimeOffset(DateTime.Now);
        //                    //check.CallbackOption = CallbackOption;
        //                    db.SaveChanges();
        //                }
        //                else
        //                {
        //                    CreateSOSAlert(sos.ML.RecepientUserId, SOSType, sos.M.MessageId, sos.ML.MessageListId, sos.ML.ResponseID, (int)sos.M.IncidentActivationId,
        //                        sos.AMR.ResponseLabel, sos.ML.UpdatedOn, DateTime.Now, sos.ML.UserLocationLat, sos.ML.UserLocationLong, CallbackOption);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        //public void CreateSOSAlert(int UserID, string SOSType, int MessageId, int MessageListId, int ResponseID, int IncidentActivationId,
        //    string ResponseLabel, DateTimeOffset UpdatedOn, DateTimeOffset ResponseTimeGMT, string Lat, string Lng, int CallbackOption)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        SOSAlert SA = new SOSAlert();
        //        SA.UserID = UserID;
        //        SA.ActiveIncidentID = IncidentActivationId;
        //        SA.AlertType = SOSType;
        //        SA.Latitude = DBC.Left(Lat, 15);
        //        SA.Longitude = DBC.Left(Lng, 15);
        //        SA.MessageID = MessageId;
        //        SA.MessageListID = MessageListId;
        //        SA.ResponseID = ResponseID;
        //        SA.ResponseLabel = ResponseLabel;
        //        SA.ResponseTime = UpdatedOn;
        //        SA.ResponseTimeGMT = ResponseTimeGMT;
        //        SA.CallbackOption = CallbackOption;
        //        SA.CompletedBy = 0;
        //        SA.Completed = false;
        //        SA.CompletedOn = DBC.DbDate();
        //        db.SOSAlert.Add(SA);
        //        db.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //    }
        //}

        public async Task<int> CreateConferenceHeader(int companyId, int createdBy, string timeZoneId, string confRoom, bool record, int objectId = 0, int messageId = 0, string objectType = "Incident")
        {
            try
            {
                int CallHeaderId = 0;
                ConferenceCallLogHeader CallHeader = new ConferenceCallLogHeader()
                {
                    ActiveIncidentId = objectId,
                    TargetObjectId = objectId,
                    TargetObjectName = objectType,
                    CompanyId = companyId,
                    CreatedBy = createdBy,
                    CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    InitiatedBy = createdBy,
                    MessageId = messageId,
                    ConfRoomName = confRoom,
                    Record = record,
                    ConfrenceStart = (DateTime)SqlDateTime.MinValue,
                    ConfrenceEnd = (DateTime)SqlDateTime.MinValue
                };
                await db.AddAsync(CallHeader);
                await  db.SaveChangesAsync();
                if (CallHeader != null)
                {
                    CallHeaderId = CallHeader.ConferenceCallId;
                }
                return CallHeaderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task<int> CreateConferenceDetail(string action, int conferenceCallId, int userId, string phoneNumber, string landline, string successCallId,
            string status, string timeZoneId, int confDetailId, string calledOn = "")
        {
            int CallDetailId = 0;
            if (action.ToUpper() == "ADD")
            {
                ConferenceCallLogDetail ConfDetail = new ConferenceCallLogDetail()
                {
                    ConferenceCallId = conferenceCallId,
                    UserId = userId,
                    PhoneNumber = phoneNumber,
                    CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    SuccessCallId = successCallId,
                    Status = status,
                    ConfJoined = (DateTime)SqlDateTime.MinValue,
                    ConfLeft = (DateTime)SqlDateTime.MinValue,
                    CalledOn = calledOn
                };
                await db.AddAsync(ConfDetail);
                await db.SaveChangesAsync();
                if (ConfDetail != null)
                {
                    CallDetailId = ConfDetail.ConferenceCallDetailId;
                }
            }
            else if ((action.ToUpper() == "UPDATE"))
            {
                var ConfDetail = await db.Set<ConferenceCallLogDetail>().Where(CD=> CD.ConferenceCallDetailId == confDetailId).FirstOrDefaultAsync();
                if (ConfDetail != null)
                {
                    ConfDetail.SuccessCallId = successCallId;
                    ConfDetail.Status = status;

                    if (!string.IsNullOrEmpty(calledOn))
                        ConfDetail.CalledOn = calledOn;
                    db.SaveChanges();
                    CallDetailId = ConfDetail.ConferenceCallDetailId;
                }
            }
            return CallDetailId;
        }

        public async Task AddUserLocation(int userID, int userDeviceID, double latitude, double longitude, string locationAddress,
            DateTimeOffset userDeviceTime, string timeZoneId, int companyID)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                if (latitude != 0 && longitude != 0)
                {
                    UserLocation UL = new UserLocation
                    {
                        UserId = userID,
                        UserDeviceId = userDeviceID,
                        Lat = DBC.Left(latitude.ToString().Replace(",", "."), 15),
                        Long = DBC.Left(longitude.ToString().Replace(",", "."), 15),
                        LocationName = locationAddress,
                        CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                        CreatedOnGMT = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                        UserDeviceTime = userDeviceTime != null ? userDeviceTime : DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
                    };
                    ;
                   await db.AddAsync(UL);
                   await db.SaveChangesAsync();

                  await  DBC.UpdateUserLocation(userID, companyID, latitude.ToString(), longitude.ToString(), TimeZoneId);
                }


                var track_me = await db.Set<TrackMe>()
                                .Where(TM=> TM.UserDeviceId == userDeviceID &&
                                TM.TrackMeStopped.Value.Year < 2000).ToListAsync();
                foreach (var tm in track_me)
                {
                    tm.LastUpdate = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public dynamic GetMessageResponseList(int CompanyID, int CurrentUserID, string TimeZoneID, int ResponseID = 0, string MessageType = "ALL", int Status = 1)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {

        //        if (ResponseID > 0)
        //        {
        //            var option = (from MR in db.CompanyMessageResponse
        //                          where MR.CompanyID == CompanyID && MR.ResponseID == ResponseID
        //                          select MR).FirstOrDefault();
        //            return option;
        //        }
        //        else
        //        {
        //            var option_list = (from MR in db.CompanyMessageResponse where MR.CompanyID == CompanyID select MR).ToList();

        //            if (Status > -1)
        //            {
        //                option_list = option_list.Where(s => s.Status == Status).ToList();
        //            }

        //            if (MessageType != "ALL")
        //            {
        //                option_list = option_list.Where(w => w.MessageType == MessageType.ToUpper()).ToList();
        //            }
        //            else
        //            {
        //                if (option_list.Count <= 0)
        //                {
        //                    CopyMessageResponse(CompanyID, CurrentUserID, TimeZoneID);
        //                    option_list = (from MR in db.CompanyMessageResponse where MR.CompanyID == CompanyID select MR).ToList();
        //                }
        //            }
        //            return option_list;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //        return null;
        //    }
        //}

        //public void CopyMessageResponse(int CompanyID, int CurrentUserId, string TimeZoneID)
        //{
        //    try
        //    {
        //        var rsps = (from MR in db.LibMessageResponse select MR).ToList();
        //        Messaging MSG = new Messaging();
        //        foreach (var rsp in rsps)
        //        {
        //            MSG.CreateMessageResponse(rsp.ResponseLabel, (bool)rsp.IsSafetyOption, rsp.MessageType,
        //                "NONE", rsp.Status, CurrentUserId, CompanyID, TimeZoneID);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<int> CreateMessageList(int messageId, int recipientId, bool isTaskRecepient, bool textUsed, bool phoneUsed, bool emailUsed, bool pushUsed,
            int currentUserId, string timeZoneId)
        {
            try
            {
                var pMessageId = new SqlParameter("@MessageID", messageId);
                var pRecipientID = new SqlParameter("@UserID", recipientId);
                var pTransportType = new SqlParameter("@TransportType", "All");
                var pDateSent = new SqlParameter("@DateSent", _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));
                var pIsTaskRecepient = new SqlParameter("@IsTaskRecepient", isTaskRecepient);
                var pTextUsed = new SqlParameter("@TextUsed", TextUsed);
                var pPhoneUsed = new SqlParameter("@PhoneUsed", PhoneUsed);
                var pEmailUsed = new SqlParameter("@EmailUsed", EmailUsed);
                var pPushUsed = new SqlParameter("@PushUsed", PushUsed);
                var pCreatedBy = new SqlParameter("@CreatedBy", currentUserId);
                var pUpdatedBy = new SqlParameter("@UpdatedBy", currentUserId);
                var pUpdatedOn = new SqlParameter("@UpdatedOn", _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));


                int MsgListId = await  db.Database.ExecuteSqlRawAsync(
                    "Pro_Add_Message_List @MessageID,@UserID,@TransportType,@DateSent,@IsTaskRecepient,@TextUsed,@PhoneUsed,@EmailUsed,@PushUsed,@CreatedBy,@UpdatedBy,@UpdatedOn",
                    pMessageId, pRecipientID, pTransportType, pDateSent, pIsTaskRecepient, pTextUsed, pPhoneUsed, pEmailUsed, pPushUsed, pCreatedBy, pUpdatedBy, pUpdatedOn
                    );
                return MsgListId;

                //MessageList tblMsgLst = new MessageList() {
                //    MessageId = MessageId,
                //    RecepientUserId = RecipientID,
                //    TransportType = "All",
                //    DateAcknowledge = DateTime.MinValue.AddYears(1900),
                //    DateDelivered = DateTime.MinValue.AddYears(1900),
                //    DateSent = DBC.GetDateTimeOffset(DateTime.Now,TimeZoneId),
                //    MessageSentStatus = 0,
                //    MessageAckStatus = 0,
                //    MessageDelvStatus = 0,
                //    IsTaskRecepient = IsTaskRecepient,
                //    CreatedBy = CurrentUserID,
                //    CreatedOn = DateTime.Now,
                //    UpdatedBy = CurrentUserID,
                //    UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now,TimeZoneId)
                //};
                //db.MessageList.Add(tblMsgLst);
                //db.SaveChanges();
                //return tblMsgLst.MessageListId;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }

        public async Task<int> SaveMessageResponse(int responseId, string responseLabel, string description, bool isSafetyResponse,
                      string safetyAckAction, string messageType, int status, int currentUserId, int companyId, string timeZoneId)
        {
            try
            {
                if (responseId > 0)
                {
                    var option = (from MR in db.Set<CompanyMessageResponse>()
                                  where MR.CompanyId == companyId && MR.ResponseId == responseId
                                  select MR).FirstOrDefault();
                    if (option != null)
                    {
                        option.ResponseLabel = responseLabel;
                        option.Description = description;
                        option.IsSafetyResponse = isSafetyResponse;
                        option.SafetyAckAction = safetyAckAction;
                        option.MessageType = messageType;
                        option.Status = status;
                        option.UpdatedBy = currentUserId;
                        option.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                        await db.SaveChangesAsync();
                        return responseId;
                    }
                }
                else
                {
                    int responseid = await CreateMessageResponse(responseLabel, isSafetyResponse, messageType, safetyAckAction, status,
            currentUserId, companyId, timeZoneId);
                    return responseid;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> CreateMessageResponse(string responseLabel, bool sosEvent, string messageType, string safetyAction, int status,
            int currentUserId, int companyId, string timeZoneId)
        {
            try
            {
                CompanyMessageResponse option = new CompanyMessageResponse();
                option.ResponseLabel = responseLabel;
                option.Description = responseLabel;
                option.IsSafetyResponse = sosEvent;
                option.SafetyAckAction = safetyAction;
                option.MessageType = messageType;
                option.Status = status;
                option.UpdatedBy = currentUserId;
                option.CompanyId = companyId;
                option.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                db.Set<CompanyMessageResponse>().Add(option);
                await db.SaveChangesAsync();
                return option.ResponseId;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public void InsertMessageTransaction(int MessageId, int MessageListId, string Method, string MessageText, int Attempt, string MsgStatus,
        //    string DeviceAddress = "", string CloudMessageId = "", bool DebugOn = false, string CommsPovider = "TWILIO")
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        try
        //        {
        //            DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now);

        //            var pMessageId = new SqlParameter("@MessageId", MessageId);
        //            var pMethodName = new SqlParameter("@MethodName", Method.ToUpper());
        //            var pAttempts = new SqlParameter("@Attempts", Attempt);
        //            var pMessageText = new SqlParameter("@MessageText", MessageText);
        //            var pStatus = new SqlParameter("@Status", MsgStatus);
        //            var pCreatedOn = new SqlParameter("@CreatedOn", dtNow);
        //            var pMessageListId = new SqlParameter("@MessageListId", MessageListId);
        //            var pCloudMessageId = new SqlParameter("@CloudMessageId", CloudMessageId);
        //            var pDeviceAddress = new SqlParameter("@DeviceAddress", DeviceAddress);
        //            var pDebugOn = new SqlParameter("@DebugOn", DebugOn);
        //            var pCommsProvider = new SqlParameter("@CommsPovider", CommsPovider);

        //            var MessageData = db.Database.ExecuteSqlCommand("Message_Transaction_Insert @MessageId, @MethodName, @Attempts, @MessageText, @Status, @CreatedOn, @MessageListId, @CloudMessageId, @DeviceAddress, @DebugOn, @CommsPovider",
        //                pMessageId, pMethodName, pAttempts, pMessageText, pStatus, pCreatedOn, pMessageListId, pCloudMessageId, pDeviceAddress, pDebugOn, pCommsProvider);

        //        }
        //        catch (Exception ex)
        //        {
        //            DBC.catchException(ex);
        //        }


        //        //MessageTransaction MessageTrans = new MessageTransaction() {
        //        //    MessageId = MessageId,
        //        //    MessageListId = MessageListId,
        //        //    MethodName = Method.ToUpper(),
        //        //    MessageText = MessageText,
        //        //    Attempts = Attempt,
        //        //    Status = MsgStatus,
        //        //    CloudMessageId = CloudMessageId,
        //        //    DeviceAddress = DeviceAddress,
        //        //    CreatedOn = DateTime.Now,
        //        //    IsBilled = (DebugOn ? true : false),
        //        //    LogCollected = ((Method == "PUSH" || Method == "EMAIL" || DebugOn == true) ? true : false)
        //        //};
        //        //db.MessageTransaction.Add(MessageTrans);
        //        //db.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //    }
        //}

        ////public void ClearQueueLocks(string ServiceHost = "", string Source = "SELF") {
        ////    DBCommon DBC = new DBCommon();
        ////    try {
        ////        if(Source == "SELF")
        ////            ServiceHost = DBC.LocalhostName();

        ////        var QueueLst = (from PQ in db.ProcessQueue where PQ.LockedBy == ServiceHost && PQ.Owner != "NOOWNER" select PQ).ToList();
        ////        if(QueueLst.Count > 0) {
        ////            foreach(var QItem in QueueLst) {
        ////                if(QItem.Status == 2) {
        ////                    QItem.Status = 1;
        ////                    QItem.CurrentAction = "UPDATEQUEUE";
        ////                } else if(QItem.Status == 1 || QItem.Status == 4) {
        ////                    QItem.Status = 0;
        ////                    DeleteDevices(QItem.MessageId);
        ////                }
        ////                QItem.Owner = "NOOWNER";
        ////                if(Source != "SELF") {
        ////                    QItem.LockedBy = Source;
        ////                }
        ////                db.SaveChanges();
        ////            }
        ////        }
        ////    } catch(DbEntityValidationException ex) {
        ////        DBC.dbExceptionLog(ex, "DbCommon", "ClearQueueLocks");
        ////    }
        ////}

        public async Task<List<UserLocation>> GetUserMovements(int UserID)
        {
           
            try
            {
                var loclist = await  db.Set<UserLocation>().Where(UL=> UL.UserId == UserID)
                    .OrderByDescending(o => o).Take(15).ToListAsync();
                return loclist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public int GetCallbackOption(string AckMethod)
        //{
        //    int CallbackOption = 3;
        //    if (AckMethod == "PHONE")
        //    {
        //        CallbackOption = 1;
        //    }
        //    else if (AckMethod == "TEXT")
        //    {
        //        CallbackOption = 1;
        //    }
        //    return CallbackOption;
        //}

        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var EarthRadius = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);  // deg2rad below
            var dLon = ToRadians(lon2 - lon1);
            var area =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var coveredarea = 2 * Math.Atan2(Math.Sqrt(area), Math.Sqrt(1 - area));
            var d = EarthRadius * coveredarea; // Distance in km
            return d;
        }

        public double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public bool UserInRange(double locLat, double locLan, double userLat, double userLan, double range)
        {
            double distance = GetDistance(locLat, locLan, userLat, userLan);
            return (distance < range ? true : false);
        }

        public async Task AddTrackMeUsers(int incidentActivationId, int messageId, int companyId)
        {
            try
            {
                //get list of track me users
                var pCompanyId = new SqlParameter("@CompanyID", companyId);

                var track_me_list = await  db.Set<TrackMeUsers>().FromSqlRaw("exec Get_Track_Me_Users @CompanyID", pCompanyId).ToListAsync();

                //get the list of all the location of the incident
                var loc_list = await (from IL in db.Set<IncidentLocation>()
                                join ILL in db.Set<IncidentLocationLibrary>() on IL.LibLocationId equals ILL.LocationId
                                where IL.IncidentActivationId == incidentActivationId
                                select ILL).ToListAsync();

                double DistanceToAlert = 100;

                double.TryParse(_DBC.GetCompanyParameter("TRACKME_DISTANCE_TO_ALERT", companyId), out DistanceToAlert);

                double distancetokm = DistanceToAlert / 1000;

                List<int> UsersToNotify = new List<int>();

                foreach (var usr in track_me_list)
                {
                    double userlat = usr.Latitude;
                    double userlng = usr.Longitude;

                    foreach (var loc in loc_list)
                    {
                        double loclat = Convert.ToDouble(loc.Lat);
                        double loclng = Convert.ToDouble(loc.Lng);

                        bool isinrange = UserInRange(loclat, loclng, userlat, userlng, distancetokm);
                        if (isinrange)
                        {
                            UsersToNotify.Add(usr.UserID);
                        }
                    }
                }

                if (UsersToNotify.Count > 0)
                {
                    AddUserToNotify(messageId, UsersToNotify);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public async  Task<bool> CalculateMessageCost(int companyId, int messageId, string MessageText)
        {
            try
            {
                bool ShouldCheckCost = false;

                List<TwilioPriceList> pricelist = await GetTwiliPriceList();
                List<MessageISDList> isdlist =await  MessageISDList(messageId);

                decimal TotalPrice = 0;

                var pricing = (from I in isdlist
                               join P in pricelist
             on I.ISDCode equals P.ISDCode
                               select new { I, P }).ToList();

                if (pricing.Count > 0)
                {
                    int SMSSegment = 1;
                    SMSSegment = _DBC.ChunkString(MessageText, 160);

                    var cpp = await db.Set<CompanyPaymentProfile>().Where(CP=> CP.CompanyId == companyId).FirstOrDefaultAsync();

                    foreach (var price in pricing)
                    {

                        if (price.I.SMSCount > 0 && price.P.ChannelType == "SMS")
                        {
                            TotalPrice += Math.Max((price.P.BasePrice * SMSSegment * cpp.TextUplift), cpp.MinimumTextRate);
                            ShouldCheckCost = true;
                        }


                        if (price.I.PhoneCount > 0 && price.P.ChannelType == "PHONE")
                        {
                            TotalPrice += Math.Max((price.P.BasePrice * cpp.PhoneUplift), cpp.MinimumPhoneRate);
                            ShouldCheckCost = true;
                        }

                    }

                    if ((cpp.CreditBalance + cpp.CreditLimit) < TotalPrice && ShouldCheckCost == true)
                    {
                       await HandleMessageMethods(messageId);
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
              
                return false;
            }
        }

        public async Task<List<TwilioPriceList>> GetTwiliPriceList()
        {
            
            try
            {
                var result = await db.Set<TwilioPriceList>().FromSqlRaw("exec Pro_Twilio_Price_ByISDCode").ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<TwilioPriceList>();
        }

        public async Task<List<MessageISDList>> MessageISDList(int messageId)
        {
          
            try
            {
                var pMessageID = new SqlParameter("@MessageID", messageId);

                var result = await  db.Set<MessageISDList>().FromSqlRaw("exec Pro_Message_GetISDCodeCount @MessageID", pMessageID).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<MessageISDList>();
        }

        public async Task HandleMessageMethods(int MessageID)
        {
        
            try
            {
                var pMessageID = new SqlParameter("@MessageID", MessageID);
                await db.Database.ExecuteSqlRawAsync("Pro_Fix_Message_Methods @MessageID", pMessageID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SavePublicAlertMessageList(string sessionId, int publicAlertId, int messageId, DateTimeOffset dateSent, int textAdded, int emailAdded, int phoneAdded)
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionId", sessionId);
                var pPublicAlertID = new SqlParameter("@PublicAlertID", publicAlertId);
                var pMessageID = new SqlParameter("@MessageID", messageId);
                var pDateSent = new SqlParameter("@DateSent", dateSent);
                var pTextAdded = new SqlParameter("@TextAdded", textAdded);
                var pEmailAdded = new SqlParameter("@EmailAdded", emailAdded);
                var pPhoneAdded = new SqlParameter("@PhoneAdded", phoneAdded);

                db.Database.ExecuteSqlRaw("EXEC Create_PublicAlert_Message_List @SessionId, @PublicAlertID, @MessageID, @DateSent, @TextAdded, @EmailAdded, @PhoneAdded",
                    pSessionId, pPublicAlertID, pMessageID, pDateSent, pTextAdded, pEmailAdded, pPhoneAdded);
            }
            catch (Exception ex)
            {
            }
        }
        public AcknowledgeReturn AcknowledgeMessage(int userID, int messageID, int messageListID, string latitude, string longitude, string ackMethod, int ResponseID, string timeZoneId)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                var pUserID = new SqlParameter("@UserID", userID);
                var pMessageID = new SqlParameter("@MessageID", messageID);
                var pMessageListID = new SqlParameter("@MessageListID", messageListID);
                var pLatitude = new SqlParameter("@Latitude", DBC.Left(latitude, 15));
                var pLongitude = new SqlParameter("@Longitude", DBC.Left(longitude, 15));
                var pMode = new SqlParameter("@Mode", ackMethod);
                var pTimestamp = new SqlParameter("@Timestamp", dtNow);
                var pResponseID = new SqlParameter("@ResponseID", ResponseID);


                var MessageData = db.Set<AcknowledgeReturn>().FromSqlRaw("exec Pro_Message_Acknowledge @UserID, @MessageID, @MessageListID, @Latitude, @Longitude, @Mode, @Timestamp, @ResponseID",
                    pUserID, pMessageID, pMessageListID, pLatitude, pLongitude, pMode, pTimestamp, pResponseID).FirstOrDefault();

                Task.Factory.StartNew(() => {
                   // CCWebSocketHelper.SendMessageCountToUsersByMessage(MessageID);
                });

                return MessageData;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }

        #region Social Integration
        public void SocialPosting(int messageId, List<string> socialHandle, int companyId)
        {
            try
            {

                var social_int = _DBC.GetSocialIntegration(companyId, "");

                var msg = (from M in db.Set<Message>() where M.MessageId == messageId select M).FirstOrDefault();

                if (msg != null)
                {
                    foreach (string Handle in socialHandle)
                    {
                        var handle = social_int.Where(w => w.AccountType == Handle).FirstOrDefault();
                        if (handle != null)
                        {
                            if (Handle == "TWITTER")
                            {
                                Task.Factory.StartNew(() => TwitterPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
                            }
                            else if (Handle == "LINKEDIN")
                            {
                                Task.Factory.StartNew(() => LinkedInPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
                            }
                            else if (Handle == "FACEBOOK")
                            {
                                Task.Factory.StartNew(() => FacekbookPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void TwitterPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
        {
            string twitterURL = "https://api.twitter.com/1.1/statuses/update.json";

            // set the oauth version and signature method
            string oauth_version = "1.0";
            string oauth_signature_method = "HMAC-SHA1";

            // create unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            System.TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            string baseString = string.Format(
                baseFormat,
                consumerKey,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp, authToken,
                oauth_version,
                Uri.EscapeDataString(messageText)
            );

            string oauth_signature = null;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(authSecret))))
            {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(twitterURL) + "&" + Uri.EscapeDataString(baseString))));
            }

            // create the request header
            string authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

            string authorizationHeader = string.Format(
                authorizationFormat,
                Uri.EscapeDataString(consumerKey),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(authToken),
                Uri.EscapeDataString(oauth_version)
            );

            HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(twitterURL);
            objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream objStream = objHttpWebRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(messageText));
                objStream.Write(content, 0, content.Length);
            }

            var responseResult = "";

            try
            {
                //success posting
                WebResponse objWebResponse = objHttpWebRequest.GetResponse();
                StreamReader objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                responseResult = objStreamReader.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {
                responseResult = "Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authorizationHeader;
            }
        }

        public void LinkedInPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
        {

        }
        public void FacekbookPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
        {

        }

        #endregion Social Integration

        public void DownloadRecording(string recordingSid, int companyId, string recordingUrl)
        {
         
            try
            {
                int RetryCount = 2;
                string ServerUploadFolder = _httpContextAccessor.HttpContext.GetServerVariable("../../tmp/");
                string hostingEnv = _DBC.Getconfig("HostingEnvironment");
                string AzureAPIShare = _DBC.Getconfig("AzureAPIShare");

                string RecordingPath = _DBC.LookupWithKey("RECORDINGS_PATH");
                int.TryParse(_DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT"), out RetryCount);
                string SavePath = @RecordingPath + companyId + "\\";

                _DBC.connectUNCPath();

                FileHandler FH = new FileHandler(db,_httpContextAccessor);

                if (FH.FileExists(recordingSid + ".mp3", AzureAPIShare, SavePath))
                {
                    return;
                }

                if (hostingEnv != "AZURE")
                {
                    if (!Directory.Exists(SavePath))
                        Directory.CreateDirectory(SavePath);
                }

                try
                {
                    WebClient Client = new WebClient();
                    bool confdownloaded = false;
                    bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                    string RoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                    for (int i = 0; i < RetryCount; i++)
                    {
                        try
                        {
                            if (SendInDirect)
                            {
                                recordingUrl = RoutingApi + "Communication/DownloadRecording?FileName=" + recordingSid;
                            }
                            else
                            {
                                recordingUrl += ".mp3";
                            }
                            Client.DownloadFile(recordingUrl, @ServerUploadFolder + recordingSid + ".mp3");
                            confdownloaded = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                              confdownloaded = false;
                        }
                    }
                    if (confdownloaded)
                    {
                        if (File.Exists(@ServerUploadFolder + recordingSid + ".mp3"))
                        {
                            using (FileStream filestream = File.OpenRead(@ServerUploadFolder + recordingSid + ".mp3"))
                            {
                                if (hostingEnv == "AZURE")
                                {
                                    var result = FH.UploadToAzure(AzureAPIShare, SavePath, recordingSid + ".mp3", filestream).Result;
                                    if (FH.FileExists(recordingSid + ".mp3", AzureAPIShare, SavePath))
                                    {
                                        CommsHelper CH = new CommsHelper(db, _httpContextAccessor);
                                        CH.DeleteRecording(recordingSid);
                                    }
                                }
                                else
                                {
                                    File.Copy(@ServerUploadFolder + recordingSid + ".mp3", SavePath + recordingSid + ".mp3");

                                    if (File.Exists(SavePath + recordingSid + ".mp3"))
                                    {
                                        CommsHelper CH = new CommsHelper(db, _httpContextAccessor);
                                        CH.DeleteRecording(recordingSid);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (WebException ex)
            {
                throw new WebException();
            }
        }

    }
}

