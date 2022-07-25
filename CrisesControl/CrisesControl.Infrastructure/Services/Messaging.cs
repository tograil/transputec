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
using System.Text;
using System.Threading.Tasks;
using MessageMethod = CrisesControl.Core.Models.MessageMethod;

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
        public Messaging(CrisesControlContext _db, IHttpContextAccessor httpContextAccessor)
        {
            db = _db;
            _httpContextAccessor = httpContextAccessor;
        }
 

        public void AddUserToNotify(int MessageId, List<int> UserID, int ActiveIncidentID = 0)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                var ins = from I in UserID
                          select new UsersToNotify
                          {
                              MessageId = MessageId,
                              UserId = I,
                              ActiveIncidentId = ActiveIncidentID
                          };

                db.AddRange(ins);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateMessage(int CompanyId, string MsgText, string MessageType, int IncidentActivationId, int Priority, int CurrentUserId,
           int Source, DateTimeOffset LocalTime, bool MultiResponse, List<AckOption> AckOptions, int Status = 0, int AssetId = 0, int ActiveIncidentTaskID = 0,
           bool TrackUser = false, bool SilentMessage = false, int[] MessageMethod = null, List<MediaAttachment> MediaAttachments = null, int ParentID = 0,
           int MessageActionType = 0)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {

                if (ParentID > 0 && IncidentActivationId == 0 && MessageType.ToUpper() == "INCIDENT")
                {
                    var parentmsg =await db.Set<Message>().Where(M=> M.MessageId == ParentID).FirstOrDefaultAsync();
                    if (parentmsg != null)
                    {
                        IncidentActivationId = parentmsg.IncidentActivationId;
                    }
                }

                Message tblMessage = new Message()
                {
                    CompanyId = CompanyId,
                    MessageText = !string.IsNullOrEmpty(MsgText) ? MsgText.Trim() : "",
                    MessageType = MessageType,
                    IncidentActivationId = IncidentActivationId,
                    Priority = Priority,
                    Status = Status,
                    CreatedBy = CurrentUserId,
                    CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    UpdatedBy = CurrentUserId,
                    UpdatedOn = LocalTime,
                    Source = Source,
                    MultiResponse = MultiResponse,
                    AssetId = AssetId,
                    CreatedTimeZone = LocalTime,
                    ActiveIncidentTaskId = ActiveIncidentTaskID,
                    TrackUser = TrackUser,
                    SilentMessage = SilentMessage,
                    AttachmentCount = 0,
                    ParentId = ParentID,
                    MessageActionType = MessageActionType,
                    CascadePlanId = CascadePlanID,
                    MessageSourceAction = MessageSourceAction
                };
                await db.AddAsync(tblMessage);
                db.SaveChanges();

                if (MultiResponse)
                    await SaveActiveMessageResponse(tblMessage.MessageId, AckOptions, IncidentActivationId);

                //Add TrackMe Users in Notification.
                if (IncidentActivationId > 0)
                    AddTrackMeUsers(IncidentActivationId, tblMessage.MessageId, CompanyId);


                if (MessageMethod != null && CascadePlanID <= 0)
                {
                    if (MessageMethod.Length > 0)
                    {
                        await ProcessMessageMethod(tblMessage.MessageId, MessageMethod, IncidentActivationId, TrackUser);
                    }
                    else
                    {

                        var commsmethod = await  db.Set<MessageMethod>()
                                           .Where(MM=> MM.ActiveIncidentId == IncidentActivationId).Select(MM=>MM.MethodId).Distinct().ToArrayAsync();
                        if (commsmethod != null)
                        {
                            ProcessMessageMethod(tblMessage.MessageId, commsmethod, IncidentActivationId, TrackUser);
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
                            ProcessMessageMethod(tblMessage.MessageId, commsmethod, IncidentActivationId, TrackUser);
                        }
                    }
                }
                else
                {
                    var incmsgid = await  db.Set<MessageMethod>().Where(w => w.ActiveIncidentId == IncidentActivationId).OrderBy(o => o.MessageId).FirstOrDefaultAsync();
                    int[] commsmethod = new int[] { };

                    if (Source == 3)
                    {
                        string comms_method = DBC.GetCompanyParameter("TASK_SYSTEM_COMMS_METHOD", CompanyId);
                        commsmethod = comms_method.Split(',').Select(int.Parse).ToList().ToArray();
                    }
                    else
                    {
                        commsmethod = (from MM in db.Set<MessageMethod>()
                                       where ((MM.ActiveIncidentId == IncidentActivationId && IncidentActivationId > 0 && ParentID == 0 &&
                                       MM.MessageId == incmsgid.MessageId) || (MM.MessageId == ParentID && ParentID > 0))
                                       select MM.MethodId).Distinct().ToArray();
                    }

                    if (commsmethod != null)
                    {
                        ProcessMessageMethod(tblMessage.MessageId, commsmethod, IncidentActivationId, TrackUser);
                    } // in the else check if the parentid is greater than 0 and commesmethod is null, take the incidentactiviation id
                    //from message table and requery the methdos from the last message.
                }

                tblMessage.Text = TextUsed;
                tblMessage.Phone = PhoneUsed;
                tblMessage.Email = EmailUsed;
                tblMessage.Push = PushUsed;
                db.SaveChanges();

                //Process message attachments
                if (MediaAttachments != null)
                   await ProcessMessageAttachmentsAsync(tblMessage.MessageId, MediaAttachments, CompanyId, CurrentUserId, TimeZoneId);

                //Convert Asset to Message Attachment
                if (AssetId > 0)
                    CreateMessageAttachment(tblMessage.MessageId, AssetId, CompanyId, CurrentUserId, TimeZoneId);

                DBC.MessageProcessLog(tblMessage.MessageId, "MESSAGE_CREATED");

                return tblMessage.MessageId;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }

        public string GetCascadeChannels(List<string> Channels)
        {
            var cas_channel = string.Join(",", Channels).Split(',').Distinct().ToList();
            return string.Join(",", cas_channel);
        }
        public async Task<bool> CanSendMessage(int CompanyId)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                var comp = await db.Set<Company>().Where(w => w.CompanyId == CompanyId).FirstOrDefaultAsync();
                if (!comp.OnTrial)
                {
                    return true;
                }
                else
                {
                    var msgs = (from MT in db.Set<MessageTransaction>()
                                join M in db.Set<Message>() on MT.MessageId equals M.MessageId
                                where M.CompanyId == CompanyId
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
        public async Task ProcessMessageAttachmentsAsync(int MessageID, List<MediaAttachment> MediaAttachments, int CompanyID, int CreatedUserID, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                string AttachmentSavePath = DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
                string AttachmentUncUser = DBC.LookupWithKey("ATTACHMENT_UNC_USER");
                string AttachmentUncPwd = DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
                string AttachmentUseUnc = DBC.LookupWithKey("ATTACHMENT_USE_UNC");
                string ServerUploadFolder = DBC.LookupWithKey("UPLOAD_PATH");

                string hostingEnv = DBC.Getconfig("HostingEnvironment");
                string apiShare = DBC.Getconfig("AzureAPIShare");

                int Count = 0;
                if (MediaAttachments != null)
                {
                    try
                    {
                        if (hostingEnv.ToUpper() != "AZURE")
                        {
                            DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);
                        }

                        FileHandler FH = new FileHandler(db, _httpContextAccessor);
                        foreach (MediaAttachment ma in MediaAttachments)
                        {

                            try
                            {
                                if (File.Exists(@ServerUploadFolder + ma.FileName))
                                {

                                    string DirectoryPath = AttachmentSavePath + CompanyID.ToString() + "\\" + ma.AttachmentType.ToString() + "\\";
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
                                CreateMediaAttachment(MessageID, ma.Title, ma.FileName, ma.OriginalFileName, ma.FileSize, ma.AttachmentType, 0, CreatedUserID, TimeZoneId);
                                Count++;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                        if (Count > 0)
                        {
                            var msg = await db.Set<Message>().Where(M=> M.MessageId == MessageID).FirstOrDefaultAsync();
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

        public async Task CreateMessageAttachment(int MessageID, int AssetId, int CompanyID, int CreatedUserID, string TimeZoneId)
        {
           DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                string AttachmentSavePath = DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
                string AttachmentUncUser = DBC.LookupWithKey("ATTACHMENT_UNC_USER");
                string AttachmentUncPwd = DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
                string AttachmentUseUnc = DBC.LookupWithKey("ATTACHMENT_USE_UNC");
                string ServerUploadFolder = DBC.LookupWithKey("UPLOAD_PATH");
                string AzureAPIShare = DBC.Getconfig("AzureAPIShare");
                string AzurePortalShare = DBC.Getconfig("AzurePortalShare");
                string HostingEnvironment = DBC.Getconfig("HostingEnvironment");

                //string SaveStorage = DBC.LookupWithKey("ATTACHMENT_STORAGE");
                int Count = 0;

                try
                {
                    var asset =await db.Set<Assets>().Where(A=> A.AssetId == AssetId).FirstOrDefaultAsync();
                    if (asset != null)
                    {
                        FileHandler FH = new FileHandler(db, _httpContextAccessor);
                        string portal = DBC.LookupWithKey("PORTAL");
                        string file_url = "uploads/" + CompanyID + "/assets/" + asset.AssetType.ToLower() + "/" + asset.AssetPath;
                        string DirectoryPath = AttachmentSavePath + CompanyID.ToString() + "\\2\\";

                        ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                        if (HostingEnvironment.ToUpper() == "AZURE")
                        {
                            string connectionString = DBC.Getconfig("AzureFileShareConnection");

                            ShareClient desti_share = new ShareClient(connectionString, AzureAPIShare);
                            ShareDirectoryClient desti_directory = desti_share.GetDirectoryClient(DirectoryPath.Replace("\\", "/"));
                            if (!desti_directory.Exists())
                            {
                                desti_directory.Create();
                            }
                            var result = FH.CopyFileAsync(AzurePortalShare, file_url, AzureAPIShare, @DirectoryPath + asset.AssetPath);

                            CreateMediaAttachment(MessageID, asset.AssetTitle, asset.AssetPath, asset.SourceFileName, (decimal)asset.AssetSize, 2, 0, CreatedUserID, TimeZoneId);
                            Count++;
                        }
                        else
                        {
                            DBC.connectUNCPath();
                            DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);

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
                                        CreateMediaAttachment(MessageID, asset.AssetTitle, asset.AssetPath, asset.SourceFileName, (decimal)asset.AssetSize, 2, 0, CreatedUserID, TimeZoneId);
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
                        var msg = await  db.Set<Message>().Where(M=> M.MessageId == MessageID ).FirstOrDefaultAsync();
                        if (msg != null)
                        {
                            msg.AttachmentCount += Count;
                            db.SaveChangesAsync();
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

        public async Task<List<MessageDetails>> GetReplies(int ParentID, int CompanyID, int UserID, string Source = "WEB")
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
               

                    var pParentID = new SqlParameter("@ParentID", ParentID);
                    var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                    var pUserID = new SqlParameter("@UserID", UserID);
                    var pSource = new SqlParameter("@Source", Source);

                var result = await db.Set<MessageDetails>().FromSqlRaw("exec Pro_User_Message_Reply @ParentID, @CompanyID, @UserID, @Source",
                    pParentID, pCompanyID, pUserID, pSource).ToListAsync();


                        result.Select(c => {
                            c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                            c.AckOptions = (from AK in db.Set<ActiveMessageResponse>()
                                            where AK.MessageId == c.MessageId
                                            orderby AK.ResponseCode
                                            select new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToList();
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

        public async Task<List<AudioAssetReturn>> GetMessageAudio(int AssetTypeID, int CompanyID, int UserID, string Source = "APP")
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                

                    var pAssetTypeID = new SqlParameter("@AssetTypeID", AssetTypeID);
                    var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                    var pUserID = new SqlParameter("@UserID", UserID);
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

        public async Task CreateIncidentNotificationList(int MessageId, int IncidentActivationId, int MappingID, int SourceID, int CurrentUserId, int CompanyId, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            IncidentNotificationList tblIncidentNotiLst = new IncidentNotificationList()
            {
                CompanyId = CompanyId,
                IncidentActivationId = IncidentActivationId,
                ObjectMappingId = MappingID,
                SourceObjectPrimaryId = SourceID,
                MessageId = MessageId,
                Status = 1,
                CreatedBy = CurrentUserId,
                CreatedOn = DateTime.Now,
                UpdatedBy = CurrentUserId,
                UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
            };
           await  db.AddAsync(tblIncidentNotiLst);
            await db.SaveChangesAsync();
        }

        public async Task<int> CreateMediaAttachment(int MessageID, string Title, string FilePath, string OriginalFileName, decimal FileSize, int AttachmentType,
            int MessageListID, int CreatedBy, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                MessageAttachment MA = new MessageAttachment
                {
                    AttachmentType = AttachmentType,
                    CreatedBy = CreatedBy,
                    CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    FilePath = FilePath,
                    FileSize = FileSize,
                    MessageId = MessageID,
                    MessageListId = MessageListID,
                    OriginalFileName = OriginalFileName,
                    Title = Title
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

        public async Task ProcessMessageMethod(int MessageID, int[] MessageMethod, int IncidentactivationID, bool TrackUser = false)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            bool pushadded = false;
            try
            {
                int pushmethodid = 1;

                var methodlist = await db.Set<CommsMethod>().ToListAsync();

                if (TrackUser)
                {
                    pushmethodid = methodlist.Where(w => w.MethodCode == "PUSH").Select(s => s.CommsMethodId).FirstOrDefault();
                }

                foreach (int Method in MessageMethod)
                {
                    CreateMessageMethod(MessageID, Method, IncidentactivationID);

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
                if (TrackUser && !pushadded)
                {
                   await  CreateMessageMethod(MessageID, pushmethodid, IncidentactivationID);
                    PushUsed = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateMessageMethod(int MessageID, int MethodID, int ActiveIncidentID = 0, int IncidentID = 0)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                var exist = await  db.Set<MessageMethod>()
                             .Where(MMS=>
                                    MMS.ActiveIncidentId == ActiveIncidentID &&
                                    ActiveIncidentID > 0 &&
                                    MMS.MethodId == MethodID).AnyAsync();

                MessageMethod MM = new MessageMethod()
                {
                    MessageId = MessageID,
                    MethodId = MethodID,
                    ActiveIncidentId = (exist == false ? ActiveIncidentID : 0),
                    IncidentId = IncidentID
                };
                await db.AddAsync(MM);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void DeleteMessageMethod(int MessageID = 0, int ActiveIncidentID = 0)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                if (ActiveIncidentID == 0 && MessageID > 0)
                {
                    var mt_list = await db.Set<MessageMethod>().Where(MM=> MM.MessageId == MessageID).ToListAsync();
                    db.RemoveRange(mt_list);
                }
                else
                {
                    var mt_list = await db.Set<MessageMethod>().Where(MM => MM.ActiveIncidentId == ActiveIncidentID).ToListAsync();
                    db.RemoveRange(mt_list);
                }
               await  db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task SaveActiveMessageResponse(int MessageID, List<AckOption> AckOptions, int IncidentActivationID = 0)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {

                //Deleting temp records
                var delete_old = await db.Set<ActiveMessageResponse>()
                                  .Where(AMR=> AMR.MessageId == 0 && AMR.ActiveIncidentId == IncidentActivationID
                                ).ToListAsync();
                db.RemoveRange(delete_old);
                await db.SaveChangesAsync();

                foreach (AckOption ao in AckOptions)
                {
                    var option = await  db.Set<CompanyMessageResponse>().Where(w => w.ResponseId == ao.ResponseId).FirstOrDefaultAsync();
                    if (option != null)
                    {
                        ActiveMessageResponse AC = new ActiveMessageResponse
                        {
                            MessageId = MessageID,
                            ResponseId = ao.ResponseId,
                            ResponseCode = ao.ResponseCode,
                            ResponseLabel = option.ResponseLabel,
                            IsSafetyResponse = option.IsSafetyResponse,
                            SafetyAckAction = option.SafetyAckAction,
                            ActiveIncidentId = IncidentActivationID
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

        public async Task CreateProcessQueue(int MessageId, string MessageType, string Method, string State, int Priority)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                var queue = await db.Set<ProcessQueue>()
                             .Where(PQ=> PQ.Method == Method && PQ.MessageId == MessageId).FirstOrDefaultAsync();
                if (queue == null)
                {
                    ProcessQueue INPQ = new ProcessQueue
                    {
                        MessageId = MessageId,
                        CreatedOn = DBC.GetDateTimeOffset(DateTime.Now),
                        Priority = Priority,
                        MessageType = MessageType,
                        Method = Method,
                        State = State
                    };
                   await  db.AddAsync(INPQ);
                   await  db.SaveChangesAsync();
                }
                else
                {
                    if (queue != null)
                    {
                        queue.State = State;
                        await db.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NotificationDetails>> MessageNotifications(int CompanyId, int CurrentUserId)
        {
            List<NotificationDetails> NDL = new List<NotificationDetails>();

            List<IIncidentMessages> IM =await _get_incident_message(CurrentUserId, CompanyId);
            List<IPingMessage> PM = await _get_ping_message(CurrentUserId, CompanyId);

            NDL.Add(new NotificationDetails { IncidentMessages = IM, PingMessage = PM });
            return NDL;
        }

        public async Task<UserMessageCountModel> MessageNotificationsCount(int UserID)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
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

        public async Task<List<IIncidentMessages>> _get_incident_message(int UserID, int CompanyId)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                

                    var pTargetUserId = new SqlParameter("@UserID", UserID);
                    var pCompanyID = new SqlParameter("@CompanyID", CompanyId);

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

        public async Task<List<IPingMessage>> _get_ping_message(int UserID, int CompanyId)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                

                    var pTargetUserId = new SqlParameter("@UserID", UserID);
                    var pCompanyID = new SqlParameter("@CompanyID", CompanyId);

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

        public async Task<bool> StartConference(List<User> UserList, int ObjectID, int CurrentUserID, int CompanyID, string TimeZoneId)
        {
        
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                string ConServiceEnable = DBC.GetCompanyParameter("CONCIERGE_SERVICE", CompanyID);

                var phone_method = (from CM in db.Set<CompanyComm>()
                                    join CO in db.Set<CommsMethod>() on CM.MethodId equals CO.CommsMethodId
                                    where CM.CompanyId == CompanyID && CO.MethodCode == "PHONE" && CM.Status == 1 && CM.ServiceStatus == true
                                    select CO).FirstOrDefault();

                if (ConServiceEnable == "true" && phone_method != null)
                {

                    if (UserList.Count > 0)
                    {
                        
                        var result = StartConferenceNew(CompanyID, CurrentUserID, UserList, TimeZoneId, ObjectID, 0);
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
        public async Task<string> StartConferenceNew(int CompanyId, int UserId, List<User> UserList, string TimeZoneId, int ActiveIncidentId = 0, int MessageId = 0, string ObjectType = "Incident")
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            
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
                string CONF_API = DBC.GetCompanyParameter("CONFERANCE_API", CompanyId);
                bool RecordConf = Convert.ToBoolean(DBC.GetCompanyParameter("RECORD_CONFERENCE", CompanyId));
                bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

                //Create instance of CommsApi choosen by company
                dynamic CommsAPI = DBC.InitComms(CONF_API);

                CommsAPI.IsConf = true;
                CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                //Get API configraiton from sysparameters
                string RetryNumberList = DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", CompanyId, FromNumber);
                List<string> FromNumberList = RetryNumberList.Split(',').ToList();

                FromNumber = FromNumberList.FirstOrDefault();
                CallBackUrl = DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                MessageXML = DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

                //Get the user list to fetch their mobile numbers
                List<int> nUList = new List<int>();
                nUList.Add(UserId);

                foreach (User cUser in UserList)
                {
                    nUList.Add(cUser.UserId);
                }

                var tmpUserList = await db.Set<User>()
                                   .Where(U=> U.CompanyId == CompanyId && nUList.Contains(U.UserId) && U.Status == 1)
                                   .Select(U=> new { UserId = U.UserId, ISD = U.Isdcode, PhoneNumber = U.MobileNo, U.Llisdcode, U.Landline }).Distinct().ToListAsync();

                Messaging MSG = new Messaging(db, _httpContextAccessor);

                //Create conference header
                int ConfHeaderId =await  MSG.CreateConferenceHeader(CompanyId, UserId, TimeZoneId, CommsAPI.ConfRoom, RecordConf, ActiveIncidentId, MessageId, ObjectType);

                string CallId = string.Empty;
                int ModeratorCDId = 0;
                string ModeratorNumber = string.Empty;
                string ModeratorLandline = string.Empty;
                string Status = string.Empty;

                //Loop through with each user and their phone number to make the call
                foreach (var uItem in tmpUserList)
                {
                    string Mobile = DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);
                    string Landline = DBC.FormatMobile(uItem.Llisdcode, uItem.Landline);

                    if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                    {
                        int CallDetaildId =await MSG.CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, Mobile, Landline, CallId.ToString(), "ADDED", TimeZoneId, 0);
                        if (uItem.UserId == UserId)
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
                    await MSG.CreateConferenceDetail("UPDATE", 0, 0, "", "", CallId.ToString(), CallStatus, "", ModeratorCDId, CalledOn);
                }
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public string MakeConferenceCall(string FromNumber, string CallBackUrl, string MessageXML, dynamic CommsAPI, out string CallId,
            string MobileNumber, string LandLineNumber, out string Status, string CalledOn)
        {
            CalledOn = "MOBILE";
            string CallStatus = string.Empty;
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            //Initiate the call to the modrator
            Task<dynamic> calltask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, MobileNumber, MessageXML, CallBackUrl));
            CommsStatus callrslt = calltask.Result;
            CallId = callrslt.CommsId;

            if (!callrslt.Status)
            {
                Status = "";
                CallStatus = CallId = DBC.Left(CallId, 50);
                if (!string.IsNullOrEmpty(LandLineNumber))
                {
                    Task<dynamic> recalltask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, LandLineNumber, MessageXML, CallBackUrl));
                    CommsStatus recallrslt = recalltask.Result;
                    if (recallrslt.Status)
                    {
                        CalledOn = "LANDLINE";
                        CallStatus = recallrslt.CurrentAction;
                        Status = recallrslt.CurrentAction;
                    }
                    else
                    {
                        CallId = recallrslt.CommsId;
                        Status = "";
                        CallStatus = CallId = DBC.Left(CallId, 50);
                    }
                }
            }
            else
            {
                Status = callrslt.CurrentAction;
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

        public async Task<int> CreateConferenceHeader(int CompanyId, int CreatedBy, string TimeZoneId, string ConfRoom, bool Record, int ObjectID = 0, int MessageId = 0, string ObjectType = "Incident")
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                int CallHeaderId = 0;
                ConferenceCallLogHeader CallHeader = new ConferenceCallLogHeader()
                {
                    ActiveIncidentId = ObjectID,
                    TargetObjectId = ObjectID,
                    TargetObjectName = ObjectType,
                    CompanyId = CompanyId,
                    CreatedBy = CreatedBy,
                    CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    InitiatedBy = CreatedBy,
                    MessageId = MessageId,
                    ConfRoomName = ConfRoom,
                    Record = Record,
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

        public async Task<int> CreateConferenceDetail(string Action, int ConferenceCallId, int UserId, string PhoneNumber, string Landline, string SuccessCallId,
            string Status, string TimeZoneId, int ConfDetailId, string CalledOn = "")
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            int CallDetailId = 0;
            if (Action.ToUpper() == "ADD")
            {
                ConferenceCallLogDetail ConfDetail = new ConferenceCallLogDetail()
                {
                    ConferenceCallId = ConferenceCallId,
                    UserId = UserId,
                    PhoneNumber = PhoneNumber,
                    CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
                    SuccessCallId = SuccessCallId,
                    Status = Status,
                    ConfJoined = (DateTime)SqlDateTime.MinValue,
                    ConfLeft = (DateTime)SqlDateTime.MinValue,
                    CalledOn = CalledOn
                };
                await db.AddAsync(ConfDetail);
                await db.SaveChangesAsync();
                if (ConfDetail != null)
                {
                    CallDetailId = ConfDetail.ConferenceCallDetailId;
                }
            }
            else if ((Action.ToUpper() == "UPDATE"))
            {
                var ConfDetail = await db.Set<ConferenceCallLogDetail>().Where(CD=> CD.ConferenceCallDetailId == ConfDetailId).FirstOrDefaultAsync();
                if (ConfDetail != null)
                {
                    ConfDetail.SuccessCallId = SuccessCallId;
                    ConfDetail.Status = Status;

                    if (!string.IsNullOrEmpty(CalledOn))
                        ConfDetail.CalledOn = CalledOn;
                    db.SaveChanges();
                    CallDetailId = ConfDetail.ConferenceCallDetailId;
                }
            }
            return CallDetailId;
        }

        //public void AddUserLocation(int UserID, int UserDeviceID, double Latitude, double Longitude, string LocationAddress,
        //    DateTimeOffset UserDeviceTime, string TimeZoneId, int CompanyID)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        if (Latitude != 0 && Longitude != 0)
        //        {
        //            UserLocation UL = new UserLocation
        //            {
        //                UserID = UserID,
        //                UserDeviceID = UserDeviceID,
        //                Latitude = Convert.ToDouble(DBC.Left(Latitude.ToString().Replace(",", "."), 15)),
        //                Longitude = Convert.ToDouble(DBC.Left(Longitude.ToString().Replace(",", "."), 15)),
        //                LocationAddress = LocationAddress,
        //                CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
        //                CreatedOnGMT = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId),
        //                UserDeviceTime = UserDeviceTime != null ? UserDeviceTime : DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
        //            };
        //            ;
        //            db.UserLocation.Add(UL);
        //            db.SaveChanges();

        //            DBC.UpdateUserLocation(UserID, CompanyID, Latitude.ToString(), Longitude.ToString(), TimeZoneId);
        //        }


        //        var track_me = (from TM in db.TrackMe
        //                        where TM.UserDeviceID == UserDeviceID &&
        //                        TM.TrackMeStopped.Value.Year < 2000
        //                        select TM).ToList();
        //        foreach (var tm in track_me)
        //        {
        //            tm.LastUpdate = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
        //        }
        //        db.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //    }
        //}

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

        public async Task<int> CreateMessageList(int MessageId, int RecipientID, bool IsTaskRecepient, bool TextUsed, bool PhoneUsed, bool EmailUsed, bool PushUsed,
            int CurrentUserID, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pRecipientID = new SqlParameter("@UserID", RecipientID);
                var pTransportType = new SqlParameter("@TransportType", "All");
                var pDateSent = new SqlParameter("@DateSent", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));
                var pIsTaskRecepient = new SqlParameter("@IsTaskRecepient", IsTaskRecepient);
                var pTextUsed = new SqlParameter("@TextUsed", TextUsed);
                var pPhoneUsed = new SqlParameter("@PhoneUsed", PhoneUsed);
                var pEmailUsed = new SqlParameter("@EmailUsed", EmailUsed);
                var pPushUsed = new SqlParameter("@PushUsed", PushUsed);
                var pCreatedBy = new SqlParameter("@CreatedBy", CurrentUserID);
                var pUpdatedBy = new SqlParameter("@UpdatedBy", CurrentUserID);
                var pUpdatedOn = new SqlParameter("@UpdatedOn", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));


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

        //public int SaveMessageResponse(int ResponseID, string ResponseLabel, string Description, bool IsSafetyResponse,
        //              string SafetyAckAction, string MessageType, int Status, int CurrentUserId, int CompanyID, string TimeZoneId)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        if (ResponseID > 0)
        //        {
        //            var option = (from MR in db.CompanyMessageResponse
        //                          where MR.CompanyID == CompanyID && MR.ResponseID == ResponseID
        //                          select MR).FirstOrDefault();
        //            if (option != null)
        //            {
        //                option.ResponseLabel = ResponseLabel;
        //                option.Description = Description;
        //                option.IsSafetyResponse = IsSafetyResponse;
        //                option.SafetyAckAction = SafetyAckAction;
        //                option.MessageType = MessageType;
        //                option.Status = Status;
        //                option.UpdatedBy = CurrentUserId;
        //                option.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
        //                db.SaveChanges(CurrentUserId, CompanyID);
        //                return ResponseID;
        //            }
        //        }
        //        else
        //        {
        //            int responseid = CreateMessageResponse(ResponseLabel, IsSafetyResponse, MessageType, SafetyAckAction, Status,
        //    CurrentUserId, CompanyID, TimeZoneId);
        //            return responseid;
        //        }
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //        return 0;
        //    }
        //}

        //public int CreateMessageResponse(string ResponseLabel, bool SOSEvent, string MessageType, string SafetyAction, int Status,
        //    int CurrentUserId, int CompanyID, string TimeZoneId)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        CompanyMessageResponse option = new CompanyMessageResponse();
        //        option.ResponseLabel = ResponseLabel;
        //        option.Description = ResponseLabel;
        //        option.IsSafetyResponse = SOSEvent;
        //        option.SafetyAckAction = SafetyAction;
        //        option.MessageType = MessageType;
        //        option.Status = Status;
        //        option.UpdatedBy = CurrentUserId;
        //        option.CompanyID = CompanyID;
        //        option.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
        //        db.CompanyMessageResponse.Add(option);
        //        db.SaveChanges(CurrentUserId, CompanyID);
        //        return option.ResponseID;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

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

        //public dynamic GetUserMovements(int UserID)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        var loclist = (from UL in db.UserLocation where UL.UserID == UserID select UL)
        //            .OrderByDescending(o => o.CreatedOn).Take(15).ToList();
        //        return loclist;
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //        return null;
        //    }
        //}

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

        public double GetDistance(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            var EarthRadius = 6371; // Radius of the earth in km
            var dLat = ToRadians(Lat2 - Lat1);  // deg2rad below
            var dLon = ToRadians(Lon2 - Lon1);
            var area =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Lat1)) * Math.Cos(ToRadians(Lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var coveredarea = 2 * Math.Atan2(Math.Sqrt(area), Math.Sqrt(1 - area));
            var d = EarthRadius * coveredarea; // Distance in km
            return d;
        }

        public double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public bool UserInRange(double LocLat, double LocLan, double UserLat, double UserLan, double Range)
        {
            double distance = GetDistance(LocLat, LocLan, UserLat, UserLan);
            return (distance < Range ? true : false);
        }

        public void AddTrackMeUsers(int IncidentActivationId, int MessageID, int CompanyId)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                //get list of track me users
                var pCompanyId = new SqlParameter("@CompanyID", CompanyId);

                var track_me_list = db.Set<TrackMeUsers>().FromSqlRaw("exec Get_Track_Me_Users @CompanyID", pCompanyId).ToList();

                //get the list of all the location of the incident
                var loc_list = (from IL in db.Set<IncidentLocation>()
                                join ILL in db.Set<IncidentLocationLibrary>() on IL.LibLocationId equals ILL.LocationId
                                where IL.IncidentActivationId == IncidentActivationId
                                select ILL).ToList();

                double DistanceToAlert = 100;

                double.TryParse(DBC.GetCompanyParameter("TRACKME_DISTANCE_TO_ALERT", CompanyId), out DistanceToAlert);

                double distancetokm = DistanceToAlert / 1000;

                Messaging MSG = new Messaging(db,_httpContextAccessor);
                List<int> UsersToNotify = new List<int>();

                foreach (var usr in track_me_list)
                {
                    double userlat = usr.Latitude;
                    double userlng = usr.Longitude;

                    foreach (var loc in loc_list)
                    {
                        double loclat = Convert.ToDouble(loc.Lat);
                        double loclng = Convert.ToDouble(loc.Lng);

                        bool isinrange = MSG.UserInRange(loclat, loclng, userlat, userlng, distancetokm);
                        if (isinrange)
                        {
                            UsersToNotify.Add(usr.UserID);
                        }
                    }
                }

                if (UsersToNotify.Count > 0)
                {
                    MSG.AddUserToNotify(MessageID, UsersToNotify);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public AcknowledgeReturn AcknowledgeMessage(int UserID, int MessageID, int MessageListID, string Latitude, string Longitude, string AckMethod, int ResponseID, string TimeZoneId)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
        //        var pUserID = new SqlParameter("@UserID", UserID);
        //        var pMessageID = new SqlParameter("@MessageID", MessageID);
        //        var pMessageListID = new SqlParameter("@MessageListID", MessageListID);
        //        var pLatitude = new SqlParameter("@Latitude", DBC.Left(Latitude, 15));
        //        var pLongitude = new SqlParameter("@Longitude", DBC.Left(Longitude, 15));
        //        var pMode = new SqlParameter("@Mode", AckMethod);
        //        var pTimestamp = new SqlParameter("@Timestamp", dtNow);
        //        var pResponseID = new SqlParameter("@ResponseID", ResponseID);


        //        var MessageData = db.Database.SqlQuery<AcknowledgeReturn>("Pro_Message_Acknowledge @UserID, @MessageID, @MessageListID, @Latitude, @Longitude, @Mode, @Timestamp, @ResponseID",
        //            pUserID, pMessageID, pMessageListID, pLatitude, pLongitude, pMode, pTimestamp, pResponseID).FirstOrDefault();

        //        return MessageData;
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //        return null;
        //    }
        //}

        public async  Task<bool> CalculateMessageCost(int CompanyID, int MessageID, string MessageText)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
                bool ShouldCheckCost = false;

                List<TwilioPriceList> pricelist = await GetTwiliPriceList();
                List<MessageISDList> isdlist =await  MessageISDList(MessageID);

                decimal TotalPrice = 0;

                var pricing = (from I in isdlist
                               join P in pricelist
             on I.ISDCode equals P.ISDCode
                               select new { I, P }).ToList();

                if (pricing.Count > 0)
                {
                    int SMSSegment = 1;
                    SMSSegment = DBC.ChunkString(MessageText, 160);

                    var cpp = await db.Set<CompanyPaymentProfile>().Where(CP=> CP.CompanyId == CompanyID).FirstOrDefaultAsync();

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
                       await HandleMessageMethods(MessageID);
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

        public async Task<List<MessageISDList>> MessageISDList(int MessageID)
        {
          
            try
            {
                var pMessageID = new SqlParameter("@MessageID", MessageID);

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

        //public void SavePublicAlertMessageList(string SessionId, int PublicAlertID, int MessageID, DateTimeOffset DateSent, int TextAdded, int EmailAdded, int PhoneAdded)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {
        //        using (CrisesControlEntities ndb = new CrisesControlEntities())
        //        {
        //            var pSessionId = new SqlParameter("@SessionId", SessionId);
        //            var pPublicAlertID = new SqlParameter("@PublicAlertID", PublicAlertID);
        //            var pMessageID = new SqlParameter("@MessageID", MessageID);
        //            var pDateSent = new SqlParameter("@DateSent", DateSent);
        //            var pTextAdded = new SqlParameter("@TextAdded", TextAdded);
        //            var pEmailAdded = new SqlParameter("@EmailAdded", EmailAdded);
        //            var pPhoneAdded = new SqlParameter("@PhoneAdded", PhoneAdded);

        //            ndb.Database.ExecuteSqlCommand("Create_PublicAlert_Message_List @SessionId, @PublicAlertID, @MessageID, @DateSent, @TextAdded, @EmailAdded, @PhoneAdded",
        //                pSessionId, pPublicAlertID, pMessageID, pDateSent, pTextAdded, pEmailAdded, pPhoneAdded);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex);
        //    }
        //}


        //#region Social Integration
        //public void SocialPosting(int MessageID, List<string> SocialHandle, int CompanyId)
        //{
        //    DBCommon DBC = new DBCommon();
        //    try
        //    {

        //        var social_int = DBC.GetSocialIntegration(CompanyId, "");

        //        var msg = (from M in db.Message where M.MessageId == MessageID select M).FirstOrDefault();

        //        if (msg != null)
        //        {
        //            foreach (string Handle in SocialHandle)
        //            {
        //                var handle = social_int.Where(w => w.AccountType == Handle).FirstOrDefault();
        //                if (handle != null)
        //                {
        //                    if (Handle == "TWITTER")
        //                    {
        //                        Task.Factory.StartNew(() => TwitterPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
        //                    }
        //                    else if (Handle == "LINKEDIN")
        //                    {
        //                        Task.Factory.StartNew(() => LinkedInPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
        //                    }
        //                    else if (Handle == "FACEBOOK")
        //                    {
        //                        Task.Factory.StartNew(() => FacekbookPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DBC.catchException(ex, "QueueHelper", "SocialPosting");
        //    }
        //}

        //public void TwitterPost(string MessageText, string ConsumerKey, string ConsumerSecret, string AuthToken, string AuthSecret)
        //{
        //    string twitterURL = "https://api.twitter.com/1.1/statuses/update.json";

        //    // set the oauth version and signature method
        //    string oauth_version = "1.0";
        //    string oauth_signature_method = "HMAC-SHA1";

        //    // create unique request details
        //    string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        //    System.TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        //    string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

        //    // create oauth signature
        //    string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

        //    string baseString = string.Format(
        //        baseFormat,
        //        ConsumerKey,
        //        oauth_nonce,
        //        oauth_signature_method,
        //        oauth_timestamp, AuthToken,
        //        oauth_version,
        //        Uri.EscapeDataString(MessageText)
        //    );

        //    string oauth_signature = null;
        //    using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(Uri.EscapeDataString(ConsumerSecret) + "&" + Uri.EscapeDataString(AuthSecret))))
        //    {
        //        oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(twitterURL) + "&" + Uri.EscapeDataString(baseString))));
        //    }

        //    // create the request header
        //    string authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

        //    string authorizationHeader = string.Format(
        //        authorizationFormat,
        //        Uri.EscapeDataString(ConsumerKey),
        //        Uri.EscapeDataString(oauth_nonce),
        //        Uri.EscapeDataString(oauth_signature),
        //        Uri.EscapeDataString(oauth_signature_method),
        //        Uri.EscapeDataString(oauth_timestamp),
        //        Uri.EscapeDataString(AuthToken),
        //        Uri.EscapeDataString(oauth_version)
        //    );

        //    HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(twitterURL);
        //    objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
        //    objHttpWebRequest.Method = "POST";
        //    objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
        //    using (Stream objStream = objHttpWebRequest.GetRequestStream())
        //    {
        //        byte[] content = ASCIIEncoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(MessageText));
        //        objStream.Write(content, 0, content.Length);
        //    }

        //    var responseResult = "";

        //    try
        //    {
        //        //success posting
        //        WebResponse objWebResponse = objHttpWebRequest.GetResponse();
        //        StreamReader objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
        //        responseResult = objStreamReader.ReadToEnd().ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        responseResult = "Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authorizationHeader;
        //    }
        //}

        //public void LinkedInPost(string MessageText, string ConsumerKey, string ConsumerSecret, string AuthToken, string AuthSecret)
        //{

        //}
        //public void FacekbookPost(string MessageText, string ConsumerKey, string ConsumerSecret, string AuthToken, string AuthSecret)
        //{

        //}

        //#endregion Social Integration

    }
}

