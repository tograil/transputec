using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using CrisesControl.Core.DBCommon.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Companies;
using System.Net;
using Azure.Storage.Files.Shares;
using Microsoft.Data.SqlClient;
using CrisesControl.Core.Users;
using CrisesControl.Core.Register;
using System.Security.Cryptography;

namespace CrisesControl.Infrastructure.Services;

public class MessageService : IMessageService
{
    //private readonly IMessageRepository _messageRepository;
    private readonly CrisesControlContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDBCommonRepository _DBC;
   // private readonly ISenderEmailService _SDE;
    bool _textUsed = false;
    bool _phoneUsed = false;
    bool _emailUsed = false;
    bool _pushUsed = false;
    string _timeZoneId = "GMT Standard Time";
    int _cascadePlanID = 0;
    string _messageSourceAction = "";
    public MessageService(/*IMessageRepository messageRepository,*/ CrisesControlContext context, IHttpContextAccessor httpContextAccessor, IDBCommonRepository DBC/*, ISenderEmailService SDE*/)
    {
      //  _messageRepository = messageRepository;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _DBC = DBC;
        //_SDE = SDE;
    }

    public bool TextUsed
    {
        get => _textUsed;
        set => _textUsed = value;
    }
    public bool PhoneUsed {
        get => _phoneUsed;
        set => _phoneUsed = value;
    }
    public bool EmailUsed
    { 
        get => _emailUsed; 
        set => _emailUsed = value;
    }
    public bool PushUsed { get => _pushUsed; set => _pushUsed = value; }

    public string TimeZoneId { get => _timeZoneId; set => _timeZoneId = value; }

    public int CascadePlanID { get => _cascadePlanID; set => _cascadePlanID = value; }
    public string MessageSourceAction { get => _messageSourceAction; set => _messageSourceAction =value; }
  

    //public Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false)
    //{
    //    throw new System.NotImplementedException();
    //}
    public async Task CreateMessageMethod(int MessageID, int MethodID, int ActiveIncidentID = 0, int IncidentID = 0)
    {
        
        try
        {
            var exist = await _context.Set<MessageMethod>()
                        .Where( MMS=>
                            MMS.ActiveIncidentId == ActiveIncidentID &&
                            ActiveIncidentID > 0 &&
                            MMS.MethodId == MethodID
                         ).AnyAsync();

            MessageMethod MM = new MessageMethod()
            {
                MessageId = MessageID,
                MethodId = MethodID,
                ActiveIncidentId = (exist == false ? ActiveIncidentID : 0),
                IncidentId = IncidentID
            };
           await _context.AddAsync(MM);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task AddUserToNotify(int messageId, List<int> userId, int activeIncidentId = 0)
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

            _context.AddRange(ins);
            await _context.SaveChangesAsync();

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
                var parentmsg = await _context.Set<Message>().Where(M => M.MessageId == parentId).FirstOrDefaultAsync();
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
                CreatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
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
                CascadePlanId = _cascadePlanID,
                MessageSourceAction = _messageSourceAction
            };
            await _context.AddAsync(tblMessage);
            await _context.SaveChangesAsync();

            if (multiResponse)
                await SaveActiveMessageResponse(tblMessage.MessageId, ackOptions, incidentActivationId);

            //Add TrackMe Users in Notification.
            if (incidentActivationId > 0)
                await AddTrackMeUsers(incidentActivationId, tblMessage.MessageId, companyId);


            if (messageMethod != null && _cascadePlanID <= 0)
            {
                if (messageMethod.Length > 0)
                {
                    await ProcessMessageMethod(tblMessage.MessageId, messageMethod, incidentActivationId, trackUser);
                }
                else
                {

                    var commsmethod = await _context.Set<CrisesControl.Core.Messages.MessageMethod>()
                                       .Where(MM => MM.ActiveIncidentId == incidentActivationId).Select(MM => MM.MethodId).Distinct().ToArrayAsync();
                    if (commsmethod != null)
                    {
                        await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                    }
                }
            }
            else if (_cascadePlanID > 0)
            {
                var methods = await _context.Set<PriorityInterval>()
                               .Where(PI => PI.CascadingPlanId == _cascadePlanID)
                               .Select(PI => PI.Methods).ToListAsync();
                if (methods != null)
                {
                    string channels =await GetCascadeChannels(methods);
                    var commsmethod = channels.Split(',').Select(int.Parse).ToArray();
                    if (commsmethod != null)
                    {
                        await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                    }
                }
            }
            else
            {
                var incmsgid = await _context.Set<MessageMethod>().Where(w => w.ActiveIncidentId == incidentActivationId).OrderBy(o => o.MessageId).FirstOrDefaultAsync();
                int[] commsmethod = new int[] { };

                if (source == 3)
                {
                    string comms_method =await _DBC.GetCompanyParameter("TASK_SYSTEM_COMMS_METHOD", companyId);
                    commsmethod = comms_method.Split(',').Select(int.Parse).ToList().ToArray();
                }
                else
                {
                    commsmethod = await (from MM in _context.Set<MessageMethod>()
                                   where ((MM.ActiveIncidentId == incidentActivationId && incidentActivationId > 0 && parentId == 0 &&
                                   MM.MessageId == incmsgid.MessageId) || (MM.MessageId == parentId && parentId > 0))
                                   select MM.MethodId).Distinct().ToArrayAsync();
                }

                if (commsmethod != null)
                {
                    await ProcessMessageMethod(tblMessage.MessageId, commsmethod, incidentActivationId, trackUser);
                } // in the else check if the parentid is greater than 0 and commesmethod is null, take the incidentactiviation id
                  //from message table and requery the methdos from the last message.
            }

            tblMessage.Text = _textUsed;
            tblMessage.Phone = _phoneUsed;
            tblMessage.Email = _emailUsed;
            tblMessage.Push = _pushUsed;
            await _context.SaveChangesAsync();

            //Process message attachments
            if (mediaAttachments != null)
                await ProcessMessageAttachments(tblMessage.MessageId, mediaAttachments, companyId, currentUserId, _timeZoneId);

            //Convert Asset to Message Attachment
            if (assetId > 0)
                await CreateMessageAttachment(tblMessage.MessageId, assetId, companyId, currentUserId, _timeZoneId);

            await _DBC.MessageProcessLog(tblMessage.MessageId, "MESSAGE_CREATED");

            return tblMessage.MessageId;
        }
        catch (Exception ex)
        {
            throw ex;
            return 0;
        }
    }

    public async Task<string> GetCascadeChannels(List<string> channels)
    {
        var cas_channel = string.Join(",", channels).Split(',').Distinct().ToList();
        return string.Join(",", cas_channel);
    }
    public async Task<bool> CanSendMessage(int companyId)
    {
        try
        {
            var comp = await _context.Set<Company>().Where(w => w.CompanyId == companyId).FirstOrDefaultAsync();
            if (!comp.OnTrial)
            {
                return true;
            }
            else
            {
                var msgs = await (from MT in _context.Set<MessageTransaction>()
                            join M in _context.Set<Message>() on MT.MessageId equals M.MessageId
                            where M.CompanyId == companyId
                            select MT).ToListAsync();
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
    public async Task ProcessMessageAttachments(int messageId, List<MediaAttachment> mediaAttachments, int companyId, int createdUserId, string timeZoneId)
    {
        try
        {
            string AttachmentSavePath = await _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
            string AttachmentUncUser = await _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
            string AttachmentUncPwd = await _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
            string AttachmentUseUnc =await _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
            string ServerUploadFolder = await _DBC.LookupWithKey("UPLOAD_PATH");

            string hostingEnv = await _DBC.Getconfig("HostingEnvironment");
            string apiShare = await _DBC.Getconfig("AzureAPIShare");

            int Count = 0;
            if (mediaAttachments != null)
            {
                try
                {
                    if (hostingEnv.ToUpper() != "AZURE")
                    {
                       await _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);
                    }

                    FileHandler FH = new FileHandler(_context, _httpContextAccessor,_DBC);
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
                            await CreateMediaAttachment(messageId, ma.Title, ma.FileName, ma.OriginalFileName, ma.FileSize, ma.AttachmentType, 0, createdUserId, _timeZoneId);
                            Count++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    if (Count > 0)
                    {
                        var msg = await _context.Set<Message>().Where(M => M.MessageId == messageId).FirstOrDefaultAsync();
                        if (msg != null)
                        {
                            msg.AttachmentCount = Count;
                            await _context.SaveChangesAsync();
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
            string AttachmentSavePath =await _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
            string AttachmentUncUser = await _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
            string AttachmentUncPwd = await _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
            string AttachmentUseUnc = await _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
            string ServerUploadFolder =await _DBC.LookupWithKey("UPLOAD_PATH");
            string AzureAPIShare = await _DBC.Getconfig("AzureAPIShare");
            string AzurePortalShare = await _DBC.Getconfig("AzurePortalShare");
            string HostingEnvironment = await _DBC.Getconfig("HostingEnvironment");

            //string SaveStorage = DBC.LookupWithKey("ATTACHMENT_STORAGE");
            int Count = 0;

            try
            {
                var asset = await _context.Set<CrisesControl.Core.Assets.Assets>().Where(A => A.AssetId == assetId).FirstOrDefaultAsync();
                if (asset != null)
                {
                    FileHandler FH = new FileHandler(_context, _httpContextAccessor,_DBC);
                    string portal = await _DBC.LookupWithKey("PORTAL");
                    string file_url = "uploads/" + companyId + "/assets/" + asset.AssetType.ToLower() + "/" + asset.AssetPath;
                    string DirectoryPath = AttachmentSavePath + companyId.ToString() + "\\2\\";

                    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                    if (HostingEnvironment.ToUpper() == "AZURE")
                    {
                        string connectionString = await _DBC.Getconfig("AzureFileShareConnection");

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
                       await _DBC.connectUNCPath();
                       await _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);

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
                                    await CreateMediaAttachment(messageId, asset.AssetTitle, asset.AssetPath, asset.SourceFileName, (decimal)asset.AssetSize, 2, 0, createdUserId, _timeZoneId);
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
                    var msg = await _context.Set<Message>().Where(M => M.MessageId == messageId).FirstOrDefaultAsync();
                    if (msg != null)
                    {
                        msg.AttachmentCount += Count;
                        _context.Update(msg);
                        await _context.SaveChangesAsync();
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

            var result = await _context.Set<MessageDetails>().FromSqlRaw("exec Pro_User_Message_Reply @ParentID, @CompanyID, @UserID, @Source",
                pParentID, pCompanyID, pUserID, pSource).ToListAsync();


            result.Select(async c => {
                c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                c.AckOptions = await (from AK in _context.Set<ActiveMessageResponse>()
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

            var result = await _context.Set<AudioAssetReturn>().FromSqlRaw("exec Pro_Get_Message_Audio @AssetTypeID, @CompanyID, @UserID, @Source",
                pAssetTypeID, pCompanyID, pUserID, pSource).ToListAsync();

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
           
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
            UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId)
        };
        await _context.AddAsync(tblIncidentNotiLst);
        await _context.SaveChangesAsync();
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
                CreatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
                FilePath = filePath,
                FileSize = fileSize,
                MessageId = messageId,
                MessageListId = messageListId,
                OriginalFileName = originalFileName,
                Title = title
            };
            await _context.AddAsync(MA);
            await _context.SaveChangesAsync();
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

            var methodlist = await _context.Set<CommsMethod>().ToListAsync();

            if (trackUser)
            {
                pushmethodid = methodlist.Where(w => w.MethodCode == "PUSH").Select(s => s.CommsMethodId).FirstOrDefault();
            }

            foreach (int Method in messageMethod)
            {
                await CreateMessageMethod(messageId, Method, incidentactivationId);

                string chkmethod = methodlist.Where(w => w.CommsMethodId == Method).Select(s => s.MethodCode).FirstOrDefault();
                if (chkmethod == "TEXT")
                    _textUsed = true;

                //var phnmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "PHONE").Select(s => s.MethodCode).Any();
                if (chkmethod == "PHONE")
                    _phoneUsed = true;

                //var emailmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "EMAIL").Select(s => s.MethodCode).Any();
                if (chkmethod == "EMAIL")
                    _emailUsed = true;

                //var pushmethod = methodlist.Where(w => w.CommsMethodId == Method && w.MethodCode == "PUSH").Select(s => s.MethodCode).Any();
                if (chkmethod == "PUSH")
                    _pushUsed = true;

                if (pushmethodid == Method)
                    pushadded = true;
            }
            if (trackUser && !pushadded)
            {
                await CreateMessageMethod(messageId, pushmethodid, incidentactivationId);
                _pushUsed = true;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

   

    public async Task DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0)
    {
        try
        {
            if (activeIncidentId == 0 && messageId > 0)
            {
                var mt_list = await _context.Set<MessageMethod>().Where(MM => MM.MessageId == messageId).ToListAsync();
                _context.RemoveRange(mt_list);
            }
            else
            {
                var mt_list = await _context.Set<MessageMethod>().Where(MM => MM.ActiveIncidentId == activeIncidentId).ToListAsync();
                _context.RemoveRange(mt_list);
            }
            await _context.SaveChangesAsync();
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
            var delete_old = await _context.Set<ActiveMessageResponse>()
                              .Where(AMR => AMR.MessageId == 0 && AMR.ActiveIncidentId == incidentActivationId
                            ).ToListAsync();
            _context.RemoveRange(delete_old);
            await _context.SaveChangesAsync();

            foreach (AckOption ao in ackOptions)
            {
                var option = await _context.Set<CompanyMessageResponse>().Where(w => w.ResponseId == ao.ResponseId).FirstOrDefaultAsync();
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
                    await _context.AddAsync(AC);
                }
            }
            await _context.SaveChangesAsync();

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
            var queue = await _context.Set<ProcessQueue>()
                         .Where(PQ => PQ.Method == method && PQ.MessageId == messageId).FirstOrDefaultAsync();
            if (queue == null)
            {
                ProcessQueue INPQ = new ProcessQueue
                {
                    MessageId = messageId,
                    CreatedOn = await _DBC.GetDateTimeOffset(DateTime.Now),
                    Priority = priority,
                    MessageType = messageType,
                    Method = method,
                    State = state
                };
                await _context.AddAsync(INPQ);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (queue != null)
                {
                    queue.State = state;
                    await _context.SaveChangesAsync();
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

        List<IIncidentMessages> IM = await _get_incident_message(currentUserId, companyId);
        List<IPingMessage> PM = await _get_ping_message(currentUserId, companyId);

        NDL.Add(new NotificationDetails { IncidentMessages = IM, PingMessage = PM });
        return NDL;
    }

    public async Task<UserMessageCountModel> MessageNotificationsCount(int UserID)
    {
        try
        {
            var pUserID = new SqlParameter("@UserID", UserID);

            var result = await _context.Set<UserMessageCountModel>().FromSqlRaw(" exec User_Get_Message_Count @UserID", pUserID).FirstOrDefaultAsync();
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

            var result = await _context.Set<IIncidentMessages>().FromSqlRaw("exec Pro_User_Incident_Notifications @UserID, @CompanyID",
                pTargetUserId, pCompanyID).ToListAsync();

            result.Select(async c => {
                c.AckOptions = await _context.Set<ActiveMessageResponse>()
                                .Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true)
                                .OrderBy(AK => AK.ResponseCode)
                                .Select(AK => new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToListAsync();
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

            var result = await _context.Set<IPingMessage>().FromSqlRaw("exec Pro_User_Ping_Notifications @UserID, @CompanyID",
                pTargetUserId, pCompanyID).ToListAsync();


            result.Select(async c => {
                c.AckOptions = await _context.Set<ActiveMessageResponse>()
                                .Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true)
                                .OrderBy(AK => AK.ResponseCode)
                                .Select(AK => new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToListAsync();
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
            string ConServiceEnable = await _DBC.GetCompanyParameter("CONCIERGE_SERVICE", companyId);

            var phone_method = await _context.Set<CompanyComm>().Include(CO => CO.CommsMethod)
                                .Where(CM => CM.CompanyId == companyId && CM.CommsMethod.MethodCode == "PHONE" && CM.Status == 1 && CM.ServiceStatus == true
                                ).FirstOrDefaultAsync();

            if (ConServiceEnable == "true" && phone_method != null)
            {

                if (userList.Count > 0)
                {

                    var result = await StartConferenceNew(companyId, currentUserId, userList, timeZoneId, objectId, 0);
                    return true;

                }
            }
            return false;
       
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
            string CONF_API = await _DBC.GetCompanyParameter("CONFERANCE_API", companyId);
            bool RecordConf = Convert.ToBoolean(await _DBC.GetCompanyParameter("RECORD_CONFERENCE", companyId));
            bool SendInDirect =await _DBC.IsTrue(await _DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

            TwilioRoutingApi =await _DBC.LookupWithKey("TWILIO_ROUTING_API");

            //Create instance of CommsApi choosen by company
            dynamic CommsAPI = _DBC.InitComms(CONF_API);

            CommsAPI.IsConf = true;
            CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            CommsAPI.SendInDirect = SendInDirect;
            CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

            //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
            //Get API configraiton from sysparameters
            string RetryNumberList =await _DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", companyId, FromNumber);
            List<string> FromNumberList = RetryNumberList.Split(',').ToList();

            FromNumber = FromNumberList.FirstOrDefault();
            CallBackUrl =await _DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
            MessageXML = await _DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

            //Get the user list to fetch their mobile numbers
            List<int> nUList = new List<int>();
            nUList.Add(userId);

            foreach (User cUser in userList)
            {
                nUList.Add(cUser.UserId);
            }

            var tmpUserList = await _context.Set<User>()
                               .Where(U => U.CompanyId == companyId && nUList.Contains(U.UserId) && U.Status == 1)
                               .Select(U => new { UserId = U.UserId, ISD = U.Isdcode, PhoneNumber = U.MobileNo, U.Llisdcode, U.Landline }).Distinct().ToListAsync();


            //Create conference header
            int ConfHeaderId = await CreateConferenceHeader(companyId, userId,  CommsAPI.ConfRoom, RecordConf, activeIncidentId, messageId, objectType);

            string CallId = string.Empty;
            int ModeratorCDId = 0;
            string ModeratorNumber = string.Empty;
            string ModeratorLandline = string.Empty;
            string Status = string.Empty;

            //Loop through with each user and their phone number to make the call
            foreach (var uItem in tmpUserList)
            {
                string Mobile =await _DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);
                string Landline = await _DBC.FormatMobile(uItem.Llisdcode, uItem.Landline);

                if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                {
                    int CallDetaildId = await CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, Mobile, Landline, CallId.ToString(), "ADDED", _timeZoneId, 0);
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

                CallStatus = await MakeConferenceCall(FromNumber, CallBackUrl, MessageXML, CommsAPI,CallId, ModeratorNumber, ModeratorLandline,  Status, CalledOn);
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

    public async Task<string> MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI,  string callId,
        string mobileNumber, string landLineNumber,string status, string calledOn)
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
                Task<dynamic> recalltask = await Task.Factory.StartNew(() => commsAPI.Call(fromNumber, landLineNumber, messageXML, callBackUrl));
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

   

    public async Task<int> CreateConferenceHeader(int companyId, int createdBy, string confRoom, bool record, int objectId = 0, int messageId = 0, string objectType = "Incident")
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
                CreatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
                InitiatedBy = createdBy,
                MessageId = messageId,
                ConfRoomName = confRoom,
                Record = record,
                ConfrenceStart = (DateTime)SqlDateTime.MinValue,
                ConfrenceEnd = (DateTime)SqlDateTime.MinValue
            };
            await _context.AddAsync(CallHeader);
            await _context.SaveChangesAsync();
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
                CreatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
                SuccessCallId = successCallId,
                Status = status,
                ConfJoined = (DateTime)SqlDateTime.MinValue,
                ConfLeft = (DateTime)SqlDateTime.MinValue,
                CalledOn = calledOn
            };
            await _context.AddAsync(ConfDetail);
            await _context.SaveChangesAsync();
            if (ConfDetail != null)
            {
                CallDetailId = ConfDetail.ConferenceCallDetailId;
            }
        }
        else if ((action.ToUpper() == "UPDATE"))
        {
            var ConfDetail = await _context.Set<ConferenceCallLogDetail>().Where(CD => CD.ConferenceCallDetailId == confDetailId).FirstOrDefaultAsync();
            if (ConfDetail != null)
            {
                ConfDetail.SuccessCallId = successCallId;
                ConfDetail.Status = status;

                if (!string.IsNullOrEmpty(calledOn))
                    ConfDetail.CalledOn = calledOn;
                await _context.SaveChangesAsync();
                CallDetailId = ConfDetail.ConferenceCallDetailId;
            }
        }
        return CallDetailId;
    }

    public async Task AddUserLocation(int userID, int userDeviceID, double latitude, double longitude, string locationAddress,
        DateTimeOffset userDeviceTime)
    {
        
        try
        {
            if (latitude != 0 && longitude != 0)
            {
                UserLocation UL = new UserLocation
                {
                    UserId = userID,
                    UserDeviceId = userDeviceID,
                    Lat = _DBC.Left(latitude.ToString().Replace(",", "."), 15),
                    Long = _DBC.Left(longitude.ToString().Replace(",", "."), 15),
                    LocationName = locationAddress,
                    //CreatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
                   // CreatedOnGMT = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId),
                   // UserDeviceTime = userDeviceTime != null ? userDeviceTime : await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId)
                };
                ;
                await _context.AddAsync(UL);
                await _context.SaveChangesAsync();

                await _DBC.UpdateUserLocation(userID,  latitude.ToString(), longitude.ToString(), _timeZoneId);
            }


            var track_me = await _context.Set<TrackMe>()
                            .Where(TM => TM.UserDeviceId == userDeviceID &&
                            TM.TrackMeStopped.Value.Year < 2000).ToListAsync();
            foreach (var tm in track_me)
            {
                tm.LastUpdate = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId);
            }
           await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

  

    public async Task<int> CreateMessageList(int messageId, int recipientId, bool isTaskRecepient, bool textUsed, bool phoneUsed, bool emailUsed, bool pushUsed,
        int currentUserId, string timeZoneId)
    {
        try
        {
            var pMessageId = new SqlParameter("@MessageID", messageId);
            var pRecipientID = new SqlParameter("@UserID", recipientId);
            var pTransportType = new SqlParameter("@TransportType", "All");
            var pDateSent = new SqlParameter("@DateSent", _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId));
            var pIsTaskRecepient = new SqlParameter("@IsTaskRecepient", isTaskRecepient);
            var pTextUsed = new SqlParameter("@TextUsed", _textUsed);
            var pPhoneUsed = new SqlParameter("@PhoneUsed", _phoneUsed);
            var pEmailUsed = new SqlParameter("@EmailUsed", _emailUsed);
            var pPushUsed = new SqlParameter("@PushUsed", _pushUsed);
            var pCreatedBy = new SqlParameter("@CreatedBy", currentUserId);
            var pUpdatedBy = new SqlParameter("@UpdatedBy", currentUserId);
            var pUpdatedOn = new SqlParameter("@UpdatedOn", _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId));


            int MsgListId = await _context.Database.ExecuteSqlRawAsync(
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
          
        }
    }

    public async Task<int> SaveMessageResponse(int responseId, string responseLabel, string description, bool isSafetyResponse,
                  string safetyAckAction, string messageType, int status, int currentUserId, int companyId, string timeZoneId)
    {
        try
        {
            if (responseId > 0)
            {
                var option = await  _context.Set<CompanyMessageResponse>()
                              .Where(MR=> MR.CompanyId == companyId && MR.ResponseId == responseId
                              ).FirstOrDefaultAsync();
                if (option != null)
                {
                    option.ResponseLabel = responseLabel;
                    option.Description = description;
                    option.IsSafetyResponse = isSafetyResponse;
                    option.SafetyAckAction = safetyAckAction;
                    option.MessageType = messageType;
                    option.Status = status;
                    option.UpdatedBy = currentUserId;
                    option.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId);
                    _context.Update(option);
                    await _context.SaveChangesAsync();
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
            option.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, _timeZoneId);
            await _context.AddAsync(option);
            await _context.SaveChangesAsync();
            return option.ResponseId;
        }
        catch (Exception)
        {

            throw;
        }
    }



    public async Task<List<UserLocation>> GetUserMovements(int UserID)
    {

        try
        {
            var loclist = await _context.Set<UserLocation>().Where(UL => UL.UserId == UserID)
                .OrderByDescending(o => o).Take(15).ToListAsync();
            return loclist;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<double> GetDistance(double lat1, double lon1, double lat2, double lon2)
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

    public async Task<bool> UserInRange(double locLat, double locLan, double userLat, double userLan, double range)
    {
        double distance =await GetDistance(locLat, locLan, userLat, userLan);
        return (distance < range ? true : false);
    }

    public async Task AddTrackMeUsers(int incidentActivationId, int messageId, int companyId)
    {
        try
        {
            //get list of track me users
            var pCompanyId = new SqlParameter("@CompanyID", companyId);

            var track_me_list = await _context.Set<TrackMeUsers>().FromSqlRaw("exec Get_Track_Me_Users @CompanyID", pCompanyId).ToListAsync();

            //get the list of all the location of the incident
            var loc_list = await (from IL in _context.Set<IncidentLocation>()
                                  join ILL in _context.Set<IncidentLocationLibrary>() on IL.LibLocationId equals ILL.LocationId
                                  where IL.IncidentActivationId == incidentActivationId
                                  select ILL).ToListAsync();

            double DistanceToAlert = 100;

            double.TryParse(await _DBC.GetCompanyParameter("TRACKME_DISTANCE_TO_ALERT", companyId), out DistanceToAlert);

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

                    bool isinrange = await UserInRange(loclat, loclng, userlat, userlng, distancetokm);
                    if (isinrange)
                    {
                        UsersToNotify.Add(usr.UserID);
                    }
                }
            }

            if (UsersToNotify.Count > 0)
            {
               await  AddUserToNotify(messageId, UsersToNotify);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    public async Task<bool> CalculateMessageCost(int companyId, int messageId, string MessageText)
    {
        try
        {
            bool ShouldCheckCost = false;

            List<TwilioPriceList> pricelist = await GetTwiliPriceList();
            List<MessageISDList> isdlist = await MessageISDList(messageId);

            decimal TotalPrice = 0;

            var pricing = (from I in isdlist
                           join P in pricelist
         on I.ISDCode equals P.ISDCode
                           select new { I, P }).ToList();

            if (pricing.Count > 0)
            {
                int SMSSegment = 1;
                SMSSegment = await _DBC.ChunkString(MessageText, 160);

                var cpp = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == companyId).FirstOrDefaultAsync();

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
            var result = await _context.Set<TwilioPriceList>().FromSqlRaw("exec Pro_Twilio_Price_ByISDCode").ToListAsync();
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

            var result = await _context.Set<MessageISDList>().FromSqlRaw("exec Pro_Message_GetISDCodeCount @MessageID", pMessageID).ToListAsync();
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
            await _context.Database.ExecuteSqlRawAsync("Pro_Fix_Message_Methods @MessageID", pMessageID);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task SavePublicAlertMessageList(string sessionId, int publicAlertId, int messageId, DateTimeOffset dateSent, int textAdded, int emailAdded, int phoneAdded)
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

           await _context.Database.ExecuteSqlRawAsync("EXEC Create_PublicAlert_Message_List @SessionId, @PublicAlertID, @MessageID, @DateSent, @TextAdded, @EmailAdded, @PhoneAdded",
                pSessionId, pPublicAlertID, pMessageID, pDateSent, pTextAdded, pEmailAdded, pPhoneAdded);
        }
        catch (Exception ex)
        {
        }
    }
    public async Task<AcknowledgeReturn> AcknowledgeMessage(int userID, int messageID, int messageListID, string latitude, string longitude, string ackMethod, int ResponseID, string timeZoneId)
    {
        
        try
        {
            DateTimeOffset dtNow =await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
            var pUserID = new SqlParameter("@UserID", userID);
            var pMessageID = new SqlParameter("@MessageID", messageID);
            var pMessageListID = new SqlParameter("@MessageListID", messageListID);
            var pLatitude = new SqlParameter("@Latitude", _DBC.Left(latitude, 15));
            var pLongitude = new SqlParameter("@Longitude", _DBC.Left(longitude, 15));
            var pMode = new SqlParameter("@Mode", ackMethod);
            var pTimestamp = new SqlParameter("@Timestamp", dtNow);
            var pResponseID = new SqlParameter("@ResponseID", ResponseID);


            var MessageData = _context.Set<AcknowledgeReturn>().FromSqlRaw("exec Pro_Message_Acknowledge @UserID, @MessageID, @MessageListID, @Latitude, @Longitude, @Mode, @Timestamp, @ResponseID",
                pUserID, pMessageID, pMessageListID, pLatitude, pLongitude, pMode, pTimestamp, pResponseID).FirstOrDefault();

            Task.Factory.StartNew(() => {
                // CCWebSocketHelper.SendMessageCountToUsersByMessage(MessageID);
            });

            return MessageData;
        }
        catch (Exception ex)
        {
            throw ex;
          
        }
    }

    #region Social Integration
    public async Task SocialPosting(int messageId, List<string> socialHandle, int companyId)
    {
        try
        {

            var social_int = await _DBC.GetSocialIntegration(companyId, "");

            var msg = await   _context.Set<Message>().Where(M=> M.MessageId == messageId).FirstOrDefaultAsync();

            if (msg != null)
            {
                foreach (string Handle in socialHandle)
                {
                    var handle = social_int.Where(w => w.AccountType == Handle).FirstOrDefault();
                    if (handle != null)
                    {
                        if (Handle == "TWITTER")
                        {
                          await  Task.Factory.StartNew(async () => await TwitterPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
                        }
                        else if (Handle == "LINKEDIN")
                        {
                           await Task.Factory.StartNew(async () => await LinkedInPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
                        }
                        else if (Handle == "FACEBOOK")
                        {
                          await  Task.Factory.StartNew(async  () => await FacekbookPost(msg.MessageText, handle.AdnlKeyOne, handle.AdnlKeyTwo, handle.AuthToken, handle.AuthSecret));
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

    public async Task  TwitterPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
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

    public async Task  LinkedInPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
    {

    }
    public async Task FacekbookPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret)
    {

    }

    #endregion Social Integration

    public async Task DownloadRecording(string recordingSid, int companyId, string recordingUrl)
    {

        try
        {
            int RetryCount = 2;
            string ServerUploadFolder = _httpContextAccessor.HttpContext.GetServerVariable("../../tmp/");
            string hostingEnv = await _DBC.Getconfig("HostingEnvironment");
            string AzureAPIShare = await _DBC.Getconfig("AzureAPIShare");

            string RecordingPath = await _DBC.LookupWithKey("RECORDINGS_PATH");
            int.TryParse(await _DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT"), out RetryCount);
            string SavePath = @RecordingPath + companyId + "\\";

           await _DBC.connectUNCPath();

            FileHandler FH = new FileHandler(_context, _httpContextAccessor,_DBC);

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
                bool SendInDirect =await _DBC.IsTrue(await _DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string RoutingApi =await _DBC.LookupWithKey("TWILIO_ROUTING_API");

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
                                    
                                    await _DBC.DeleteRecording(recordingSid);
                                }
                            }
                            else
                            {
                                File.Copy(@ServerUploadFolder + recordingSid + ".mp3", SavePath + recordingSid + ".mp3");

                                if (File.Exists(SavePath + recordingSid + ".mp3"))
                                {
                                  
                                   await _DBC.DeleteRecording(recordingSid);
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