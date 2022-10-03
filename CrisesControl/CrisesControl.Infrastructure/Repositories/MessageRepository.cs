using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Import;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using MessageMethod = CrisesControl.Core.Messages.MessageMethod;
using Group = CrisesControl.Core.Groups.Group;
using CrisesControl.Core.Register;

namespace CrisesControl.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{

    private int CurrentUserID;
    private int CurrentCompanyID;
    private readonly string TimeZoneId = "GMT Standard Time";
    private readonly string ServerUploadFolder = "C:\\Temp\\";

    private readonly CrisesControlContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MessageRepository> _logger;
    private  DBCommon _DBC;
    private CommsHelper _CH;
    private  PingHelper _PH;
    private Messaging _MSG;
    private  SendEmail _SDE;


    public MessageRepository(
        CrisesControlContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MessageRepository> logger       
      
        )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
       // _companyParameters = companyParameters;
        _DBC = new DBCommon(_context,_httpContextAccessor);
        _MSG = new Messaging(_context,_httpContextAccessor);
        _SDE = new SendEmail(_context,_DBC);
        _CH = new CommsHelper(_context,_httpContextAccessor);
        _PH = new PingHelper(_context, _httpContextAccessor);

        CurrentCompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        CurrentUserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
    }

    public async Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0)
    {
        var exists = await _context.Set<MessageMethod>().AnyAsync(x => activeIncidentId > 0
                                                            && x.ActiveIncidentId == activeIncidentId
                                                            && x.MethodId == methodId);

        var mm = new MessageMethod
        {
            MessageId = messageId,
            MethodId = methodId,
            ActiveIncidentId = !exists ? activeIncidentId : 0,
            IncidentId = incidentId
        };

        await _context.AddAsync(mm);
        await _context.SaveChangesAsync();
    }

    public int GetPushMethodId()
    {
        return _context.Set<CommsMethod>()
            .Where(w => w.MethodCode == "PUSH")
            .Select(s => s.CommsMethodId).First();
    }

    public async Task AddUserToNotify(int messageId, ICollection<int> userIds, int activeIncidentId = 0)
    {
        var ins = userIds.Select(x => new UsersToNotify
        {
            MessageId = messageId,
            UserId = x,
            ActiveIncidentId = activeIncidentId
        }).ToList();

        await _context.AddRangeAsync(ins);
        await _context.SaveChangesAsync();
    }

    public async Task SaveActiveMessageResponse(int messageId, ICollection<AckOption> ackOptions, int activeIncidentId = 0)
    {
        var deleteOld = await _context.Set<ActiveMessageResponse>()
            .Where(x => x.MessageId == 0 && x.ActiveIncidentId == activeIncidentId).ToListAsync();

        _context.Set<ActiveMessageResponse>().RemoveRange(deleteOld);
        await _context.SaveChangesAsync();

        foreach (var (responseId, _, responseCode) in ackOptions)
        {
            var option = _context
                .Set<CompanyMessageResponse>().FirstOrDefault(w => w.ResponseId == responseId);

            if (option is not null)
            {
                var ac = new ActiveMessageResponse
                {
                    MessageId = messageId,
                    ResponseId = responseId,
                    ResponseCode = responseCode,
                    ResponseLabel = option.ResponseLabel,
                    IsSafetyResponse = option.IsSafetyResponse,
                    SafetyAckAction = option.SafetyAckAction,
                    ActiveIncidentId = activeIncidentId
                };
                _context.Add(ac);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0)
    {
        if (activeIncidentId == 0 && messageId > 0)
        {
            var mtList = await _context.Set<MessageMethod>().Where(m => m.MessageId == messageId).ToListAsync();

            _context.RemoveRange(mtList);
        }
        else
        {
            var mtList = await _context.Set<MessageMethod>().Where(m => m.ActiveIncidentId == activeIncidentId).ToListAsync();

            _context.RemoveRange(mtList);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateMessage(int companyId, string? msgText, string messageType, int incidentActivationId, int priority,
        int currentUserId, int source, DateTimeOffset localTime, bool multiResponse, ICollection<AckOption> ackOptions, int status = 0,
        int assetId = 0, int activeIncidentTaskId = 0, bool trackUser = false, bool silentMessage = false,
        int[] messageMethod = null, ICollection<MediaAttachment> mediaAttachments = null, int parentId = 0, int messageActionType = 0)
    {
        if (parentId > 0 && incidentActivationId == 0 && messageType.ToUpper() == MethodType.Incident.ToDbMethodString())
        {
            var parentMsg = await _context.Set<Message>().FirstOrDefaultAsync(x => x.MessageId == parentId);

            if (parentMsg is not null)
            {
                incidentActivationId = parentMsg.IncidentActivationId;
            }
        }

        var saveMessage = new Message
        {
            CompanyId = companyId,
            MessageText = !string.IsNullOrEmpty(msgText) ? msgText.Trim() : string.Empty,
            MessageType = messageType,
            IncidentActivationId = incidentActivationId,
            Priority = priority,
            Status = status,
            CreatedBy = currentUserId,
            CreatedOn = DateTime.Now.GetDateTimeOffset(),
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
            MessageActionType = messageActionType,
            CascadePlanId = 0,
            MessageSourceAction = string.Empty
        };
        _context.Add(saveMessage);

        await _context.SaveChangesAsync();

        if (multiResponse)
            await SaveActiveMessageResponse(saveMessage.MessageId, ackOptions, incidentActivationId);

        return saveMessage.MessageId;
    }

    public async Task CreateIncidentNotificationList(int incidentActivationId, int messageId,
        ICollection<IncidentNotificationObjLst> launchIncidentNotificationObjLst, int currentUserId, int companyId)
    {
        var oldNotifyList = await _context.Set<IncidentNotificationList>()
            .Where(x => x.CompanyId == companyId && x.IncidentActivationId == incidentActivationId)
            .ToListAsync();

        var toDeleteList = new List<int>();
        foreach (var incidentNotificationList in launchIncidentNotificationObjLst)
        {
            if (incidentNotificationList.ObjectMappingId > 0)
            {
                var isExists = oldNotifyList
                    .FirstOrDefault(s => s.CompanyId == companyId
                                         && s.IncidentActivationId == incidentActivationId
                                         && s.ObjectMappingId == incidentNotificationList.ObjectMappingId
                                         && s.SourceObjectPrimaryId == incidentNotificationList.SourceObjectPrimaryId
                                         && s.Status == 1);
                if (isExists is not null)
                {
                    toDeleteList.Add(isExists.IncidentNotificationListId);
                }
                else
                {
                    await CreateIncidentNotificationList(messageId, incidentActivationId,
                        incidentNotificationList.ObjectMappingId,
                        incidentNotificationList.SourceObjectPrimaryId, currentUserId, companyId);
                }
            }
        }

        foreach (var incidentNotificationList in oldNotifyList)
        {
            var isDel = toDeleteList.Any(s => s == incidentNotificationList.IncidentNotificationListId);
            if (!isDel)
            {
                _context.Remove(incidentNotificationList);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreateIncidentNotificationList(int messageId, int incidentActivationId, int mappingId, int sourceId,
        int currentUserId, int companyId)
    {
        var incidentNotificationList = new IncidentNotificationList
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
            UpdatedOn = DateTime.Now.GetDateTimeOffset()
        };

        await _context.AddAsync(incidentNotificationList);
        await _context.SaveChangesAsync();
    }

    public async Task<UserMessageCount> GetNotificationsCount(int userId)
    {
        UserMessageCount result = new UserMessageCount();
        try
        {
            var pUserId = new SqlParameter("@UserID", userId);
            var response = _context.Set<UserMessageCount>().FromSqlRaw("exec User_Get_Message_Count {0}", pUserId).ToList().FirstOrDefault();
            if (response != null)
                return response;
        }
        catch (Exception ex)
        {

        }
        return result;
    }

    public async Task<CompanyMessageResponse> GetMessageResponse(int responseID, string messageType)
    {
        CompanyMessageResponse companyMessageResponse = new CompanyMessageResponse();
        try
        {

            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var pResponseID = new SqlParameter("@ResponseID", responseID.ToString());
            var pmessageType = new SqlParameter("@MessageType", messageType);
            var pStatus = new SqlParameter("@Status", -1);

            var option = _context.Set<CompanyMessageResponse>().FromSqlRaw("exec Pro_Message_Response_Select {0},{1},{2},{3}",
                pCompanyID, pResponseID, pmessageType, pStatus).ToList().FirstOrDefault();

            if (option != null)
                return option;

        }
        catch (Exception)
        {

            throw;
        }
        return companyMessageResponse;
    }

    public async Task<List<CompanyMessageResponse>> GetMessageResponses(string messageType, int Status = 1)
    {
        try
        {

            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var pResponseID = new SqlParameter("@ResponseID", "0");
            var pmessageType = new SqlParameter("@MessageType", messageType);
            var pStatus = new SqlParameter("@Status", Status);

            var option_list = await _context.Set<CompanyMessageResponse>().FromSqlRaw("exec Pro_Message_Response_Select {0},{1},{2},{3}",
                pCompanyID, pResponseID, pmessageType, pStatus).ToListAsync();

            return option_list;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<List<LibMessageResponse>> GetLibMessageResponse()
    {
        try
        {
            var rsps = (from MR in _context.Set<LibMessageResponse>() select MR).ToList();
            return rsps;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task CopyMessageResponse(int companyID, int currentUserId, string timeZoneID, CancellationToken token)
    {
        try
        {
            var rsps = await GetLibMessageResponse();
            foreach (var rsp in rsps)
            {
                int responseid = await CreateMessageResponse(rsp.ResponseLabel, (bool)rsp.IsSafetyOption, rsp.MessageType,
                    "NONE", rsp.Status, currentUserId, CurrentCompanyID, timeZoneID, token);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<int> CreateMessageResponse(string responseLabel, bool sosEvent, string messageType, string safetyAction, int status,
            int currentUserId, int companyID, string timeZoneId, CancellationToken token)
    {
        try
        {

            CompanyMessageResponse msgResponse = new CompanyMessageResponse();
            msgResponse.ResponseLabel = responseLabel;
            msgResponse.Description = responseLabel;
            msgResponse.IsSafetyResponse = sosEvent;
            msgResponse.SafetyAckAction = safetyAction;
            msgResponse.MessageType = messageType;
            msgResponse.Status = status;
            msgResponse.UpdatedBy = currentUserId;
            msgResponse.CompanyId = companyID;
            msgResponse.UpdatedOn = DateTimeOffset.UtcNow;

            await _context.AddAsync(msgResponse, token);

            await _context.SaveChangesAsync(token);

            return msgResponse.ResponseId;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<List<UserMessageList>> GetMessages(int targetUserId, string? messageType, int incidentActivationId)
    {
        try
        {

            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var pUserId = new SqlParameter("@UserID", targetUserId);
            var pmessageType = new SqlParameter("@MessageType", messageType);
            var pIncidentId = new SqlParameter("@IncidentActivationId", incidentActivationId);

            var result = await _context.Set<UserMessageList>().FromSqlRaw("exec Pro_User_Ping {0},{1},{2},{3}",
                pCompanyID, pUserId, pmessageType, pIncidentId).ToListAsync();

            return result;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<AcknowledgeReturn> AcknowledgeMessage(int userID, int messageID, int messageListID, string latitude, string longitude, string ackMethod, int responseID, string timeZoneId)
    {
        try
        {
            DateTimeOffset dtNow = DateTime.Now.GetDateTimeOffset(timeZoneId);
            var pUserID = new SqlParameter("@UserID", userID);
            var pMessageID = new SqlParameter("@MessageID", messageID);
            var pMessageListID = new SqlParameter("@MessageListID", messageListID);
            var pLatitude = new SqlParameter("@Latitude", latitude ?? string.Empty);
            var pLongitude = new SqlParameter("@Longitude", longitude ?? string.Empty);
            var pMode = new SqlParameter("@Mode", ackMethod ?? string.Empty);
            var pTimestamp = new SqlParameter("@Timestamp", dtNow);
            var pResponseID = new SqlParameter("@ResponseID", responseID);


            var MessageData = _context.Set<AcknowledgeReturn>().FromSqlRaw("exec Pro_Message_Acknowledge @UserID,@MessageID,@MessageListID, @Latitude, @Longitude,@Mode,@Timestamp,@ResponseID",
                                             pUserID, pMessageID, pMessageListID, pLatitude, pLongitude, pMode, pTimestamp, pResponseID).AsEnumerable();
            if (MessageData != null)
            {
                //Todo
                //Task.Factory.StartNew(() => {
                //    CCWebSocketHelper.SendMessageCountToUsersByMessage(messageID);
                //});

                return MessageData.FirstOrDefault();
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);


        }
        return new AcknowledgeReturn { };
    }

    public async Task<MessageAckDetails> MessageAcknowledged(int companyId, int msgListId, string timeZoneId, string userLocationLat, string userLocationLong, int currentUserId, int responseID = 0, string ackMethod = "WEB")
    {
        try
        {
            const string message = "Acknowleged";
            string ackMethodLink = (ackMethod.ToUpper() == "SMSLINK" ? "TEXT" : ackMethod);

            AcknowledgeReturn MessageData = await AcknowledgeMessage(currentUserId, 0, msgListId, userLocationLat, userLocationLong, ackMethodLink, responseID, timeZoneId);

            if (MessageData != null)
            {

                //if (ackMethod.ToUpper() == "SMSLINK")
                //    SendConfirmationText(companyId, currentUserId, msgListId);

                if (responseID > 0) {
                    int CallbackOption = await GetCallbackOption(ackMethodLink);
                    await CheckSOSAlert(msgListId, "ACKNOWLEGE", CallbackOption);
                }

                if (ackMethodLink != "APP") {
                    AddUserTrackingDevices(currentUserId);
                }

                var list = await _context.Set<MessageList>().Where(MsgList => MsgList.MessageListId == msgListId).Select(n =>
                new MessageAckDetails()
                {
                    MessageListId = n.MessageListId,
                    ErrorId = 0,
                    ErrorCode = "E117",
                    Message = message
                }).FirstOrDefaultAsync();
                return list;
            }
            else
            {
                var msg = (await _context.Set<MessageList>().Where(MsgList => MsgList.MessageListId == msgListId).Select(x =>
                 new MessageAckDetails
                 {
                     MessageListId = x.MessageListId,
                     ErrorId = 100,
                     ErrorCode = "150",
                     Message = "Message aleady acknowledged"
                 }).FirstOrDefaultAsync());
                return msg;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

        }
        return new MessageAckDetails();
    }

    public async Task<int> GetCallbackOption(string ackMethod)
    {
        int CallbackOption = 3;
        if (ackMethod == MessageType.Email.ToDbString())
        {
            CallbackOption = 1;
        }
        else if (ackMethod == MessageType.Text.ToString())
        {
            CallbackOption = 1;
        }
        return CallbackOption;
    }
    public async void AddUserTrackingDevices(int userID, int messageListID = 0)
    {

        var devices = await _context.Set<UserDevice>().Where(UD => UD.UserId == userID).ToListAsync();
        foreach (var device in devices)
        {
            if (device.DeviceType.ToUpper().Contains(Enum.GetName(typeof(DeviceType), DeviceType.ANDROID)) || device.DeviceType.ToUpper().Contains(Enum.GetName(typeof(DeviceType), DeviceType.WINDOWS)))
               await AddTrackingDevice(device.CompanyId, device.UserDeviceId, device.DeviceId, device.DeviceType, messageListID);
        }
    }
    public async Task AddTrackingDevice(int companyID, int userDeviceID, string deviceAddress, string deviceType, int messageListID = 0)
    {
        const string messageText = "Track Device";
        try
        {
            MessageDevice MessageDev = new MessageDevice();
            MessageDev.CompanyId = companyID;
            MessageDev.MessageId = 0;
            MessageDev.MessageListId = messageListID;
            MessageDev.UserDeviceId = userDeviceID;
            MessageDev.Method = Enum.GetName(typeof(SharedKernel.Enums.MessageMethod), SharedKernel.Enums.MessageMethod.Push);
            MessageDev.AttributeId = 0;
            MessageDev.MessageText = messageText;
            MessageDev.Priority = 100;
            MessageDev.Attempt = 0;
            MessageDev.Status = "PENDING";
            MessageDev.SirenOn = false;
            MessageDev.OverrideSilent = false;
            MessageDev.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            MessageDev.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            MessageDev.CreatedBy = 0;
            MessageDev.UpdatedBy = 0;
            MessageDev.DateSent = SqlDateTime.MinValue.Value;
            MessageDev.DateDelivered = SqlDateTime.MinValue.Value;
            MessageDev.DeviceAddress = deviceAddress;
            MessageDev.DeviceType = deviceType;
            await _context.AddAsync(MessageDev);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
    }

    public async void SendConfirmationText(int CompanyId, int UserId, int MessageListId) {
        
        try {
            var user = (from U in _context.Set<User>() where U.UserId == UserId && U.CompanyId == CompanyId select U).FirstOrDefault();
            if (user != null) {

                string SMS_API = _DBC.GetCompanyParameter("SMS_API", CompanyId);
                string CoPilotSid = _DBC.GetCompanyParameter("MESSAGING_COPILOT_SID", CompanyId);
                string FromNumber = _DBC.GetCompanyParameter(SMS_API + "_FROM_NUMBER", CompanyId);
                string TextMessageXML = _DBC.LookupWithKey(SMS_API + "_SMS_CALLBACK_URL");

                string sendDirect = _DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION");

                bool SendInDirect = _DBC.IsTrue(sendDirect, false);

                dynamic CommsAPI = _CH.InitComms(SMS_API);

                CommsAPI.IsConf = false;
                CommsAPI.CommsDebug = false;
                CommsAPI.USE_MESSAGING_COPILOT = SMS_API == "TWILIO" ? true : false;
                CommsAPI.MESSAGING_COPILOT_SID = CoPilotSid;
                CommsAPI.SendInDirect = SendInDirect;

                CommsStatus textrslt = new CommsStatus();

                string toNumber = _DBC.FormatMobile(user.Isdcode, user.MobileNo);
                //Change this message in TwilioSMSAck.ashx
                string message = "CRISES CONTROL: Message [" + MessageListId + "] has been acknowledged successfully.";

                textrslt = await CommsAPI.Text(FromNumber, toNumber, message, TextMessageXML);
            }

        } catch (Exception ex) {

            throw;
        }
    }
    public async Task CheckSOSAlert(int messageListID, string sosType, int callbackOption)
    {
        try
        {
            const string key = "SOS_ENABLED";
            var sos = await _context.Set<MessageList>().Include(M => M.Message).Include(AMR => AMR.ActiveMessageResponse).Where(ML => ML.MessageListId == messageListID).FirstOrDefaultAsync();

            if (sos != null)
            {
                bool IsSOSEnabled = false;
                bool.TryParse( _DBC.GetCompanyParameter(key, sos.Message.CompanyId), out IsSOSEnabled);

                if (sos.ActiveMessageResponse.IsSafetyResponse && IsSOSEnabled)
                {

                    var check = await _context.Set<Sosalert>().Where(SA => SA.UserId == sos.RecepientUserId && (SA.ActiveIncidentId == sos.Message.IncidentActivationId &&
         SA.ActiveIncidentId != 0 && sos.Message.IncidentActivationId != 0)).FirstOrDefaultAsync();
                    if (check != null)
                    {
                        check.ActiveIncidentId = sos.Message.IncidentActivationId;
                        check.AlertType = sosType ?? string.Empty;
                        check.Latitude = sos.UserLocationLat.Left(15) ?? string.Empty.Left(0);
                        check.Longitude = (sos.UserLocationLong.Left(15) ?? string.Empty.Left(0));
                        check.MessageId = sos.Message.MessageId;
                        check.MessageListId = sos.MessageListId;
                        check.ResponseId = sos.ResponseId;
                        check.ResponseLabel = sos.ActiveMessageResponse.ResponseLabel;
                        check.ResponseTime = sos.UpdatedOn;
                        check.ResponseTimeGmt = SharedKernel.Utils.DateTimeExtensions.GetDateTimeOffset(DateTime.Now);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                      await  CreateSOSAlert(sos.RecepientUserId, sosType, sos.Message.MessageId, sos.MessageListId, sos.ResponseId, (int)sos.Message.IncidentActivationId,
                            sos.ActiveMessageResponse.ResponseLabel ?? string.Empty, sos.UpdatedOn, DateTime.Now, sos.UserLocationLat ?? string.Empty, sos.UserLocationLong ?? String.Empty, callbackOption);
                    }
                }
            }
        }
        catch (Exception)
        {

        }
    }
    public async Task CreateSOSAlert(int userID, string sosType, int messageId, int messageListId, int responseID, int incidentActivationId,
            string responseLabel, DateTimeOffset updatedOn, DateTimeOffset responseTimeGMT, string lat, string lng, int callbackOption)
    {

        try
        {
            Sosalert SA = new Sosalert();
            SA.UserId = userID;
            SA.ActiveIncidentId = incidentActivationId;
            SA.AlertType = sosType;
            SA.Latitude = lat.Left(15);
            SA.Longitude = lng.Left(15);
            SA.MessageId = messageId;
            SA.MessageListId = messageListId;
            SA.ResponseId = responseID;
            SA.ResponseLabel = responseLabel;
            SA.ResponseTime = updatedOn;
            SA.ResponseTimeGmt = responseTimeGMT;
            SA.CallbackOption = callbackOption;
            SA.CompletedBy = 0;
            SA.Completed = false;
            SA.CompletedOn = SharedKernel.Utils.DateTimeExtensions.DbDate();
            await _context.AddAsync(SA);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
    }



    public async Task<NotificationDetails> MessageNotifications(int companyId, int currentUserId)
    {
        NotificationDetails NDL = new NotificationDetails();

        List<IIncidentMessages> IM = await _get_incident_message(companyId, currentUserId);
        NDL.IncidentMessages = IM;

        List<IPingMessage> PM = await _get_ping_message(companyId, currentUserId);
        NDL.PingMessage = PM;

        return NDL;
    }
    public async Task<string> LookupWithKey(string key, string Default = "")
    {
        try
        {
            Dictionary<string, string> Globals = CCConstants.GlobalVars;
            if (Globals.ContainsKey(key))
            {
                return Globals[key];
            }


            var LKP = await _context.Set<SysParameter>().Where(w => w.Name == key).FirstOrDefaultAsync();
            if (LKP != null)
            {
                Default = LKP.Value;
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

    public async Task<List<IIncidentMessages>> _get_incident_message(int companyId, int currentUserId)
    {
        var pTargetUserId = new SqlParameter("@UserID", currentUserId);
        var pCompanyID = new SqlParameter("@CompanyID", companyId);

        var result = _context.Set<IIncidentMessages>().FromSqlRaw(" exec Pro_User_Incident_Notifications @UserID, @CompanyID",
            pTargetUserId, pCompanyID).ToListAsync().Result.Select(c => {
                c.AckOptions = new List<AckOption>(_context.Set<ActiveMessageResponse>().Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true).Select(n =>
                   new AckOption() {
                       ResponseId = n.ResponseId,
                       ResponseLabel = n.ResponseLabel,
                   }).ToList());
                return c;
                }
            ).ToList();

        return result;
    }

    public async Task<List<IPingMessage>> _get_ping_message(int companyId, int currentUserId)
    {

        var pTargetUserId = new SqlParameter("@UserID", currentUserId);
        var pCompanyID = new SqlParameter("@CompanyID", companyId);

        var result = _context.Set<IPingMessage>().FromSqlRaw("exec Pro_User_Ping_Notifications @UserID, @CompanyID",
            pTargetUserId, pCompanyID).ToListAsync().Result.Select(c =>
            {
                c.AckOptions = new List<AckOption>(_context.Set<ActiveMessageResponse>().Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true).Select(n =>
                      new AckOption() {
                          ResponseId = n.ResponseId,
                          ResponseLabel = n.ResponseLabel,
                      }).ToList());
                return c;
            }).ToList();

        return result;

    }

    public async Task<IncidentMessageDetails> GetMessageDetails(string cloudMsgId, int messageId = 0)
    {
        
        var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
        var pUserId = new SqlParameter("@UserID", CurrentUserID);

        //int MsgListid = Base64Decode(CloudMsgId);
        var MsgListid = await _context.Set<MessageDevice>().FirstOrDefaultAsync(ml => ml.CloudMessageId == cloudMsgId);
        try
        {
            if (messageId > 0)
            {
                var msglistid = await _context.Set<MessageList>().FirstOrDefaultAsync(ml => ml.MessageListId == MsgListid.MessageListId);
                if (msglistid != null)
                {
                    var newmsglistid = await _context.Set<MessageList>().FirstOrDefaultAsync(ML => ML.MessageId == messageId && ML.RecepientUserId == msglistid.RecepientUserId && ML.MessageAckStatus == 0);

                    if (newmsglistid != null)
                    {
                        MsgListid.MessageListId = newmsglistid.MessageListId;
                    }

                    var CheckType = await _context.Set<MessageList>().Include(ML => ML.Message).FirstOrDefaultAsync(ML => ML.MessageListId == MsgListid.MessageListId && ML.MessageId == ML.Message.MessageId);

                    //Read sql parameters

                    var ActiveIncidentID = new SqlParameter("@ActiveIncidentID", CheckType.Message.IncidentActivationId);
                    var RecepientUserId = new SqlParameter("@UserID", CheckType.RecepientUserId);
                    var CompanyId = new SqlParameter("@CompanyID", CheckType.Message.CompanyId);
                    var MessageId = new SqlParameter("@MessageID", CheckType.MessageId);


                    if (CheckType != null)
                    {
                        if (CheckType.Message.MessageType == Enum.GetName(typeof(MessageCheckType), MessageCheckType.Incident))
                        {
                            var IncidentMessageDetails = await _context.Set<IncidentMessageDetails>().FromSqlRaw("exec Pro_Get_Incident_Message_List @ActiveIncidentID,@UserID, @CompanyID", ActiveIncidentID, RecepientUserId, CompanyId).FirstOrDefaultAsync();
                            return IncidentMessageDetails;
                        }
                        else if (CheckType.Message.MessageType == Enum.GetName(typeof(MessageCheckType), MessageCheckType.Ping))
                        {
                            var PingMessageDetails = await _context.Set<IncidentMessageDetails>().FromSqlRaw("exec Pro_Get_Ping_Message_List @UserID,@CompanyID, @MessageID, @IncidentActivationID", RecepientUserId, CompanyId, MessageId, ActiveIncidentID).FirstOrDefaultAsync();
                            return PingMessageDetails;
                        }


                    }

                }
            }


        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
        return new IncidentMessageDetails { };
    }

    public async Task<List<MessageAttachment>> GetMessageAttachment(int messageListID, int messageID)
    {
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var messageListId = new SqlParameter("@MessageListID", messageListID);
            var messageId = new SqlParameter("@MessageID", messageID);
            var attachment = await _context.Set<MessageAttachment>().FromSqlRaw("exec Pro_Get_Message_Attachment @MessageListID,@MessageID,@CompanyID", messageListId, messageId, pCompanyID).ToListAsync();
            return attachment;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                       ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
        return new List<MessageAttachment>();

    }

    public async Task<List<MessageAttachment>> GetAttachment(int messageAttachmentID)
    {

        try
        {
            var attachemntId = new SqlParameter("@MessageAttachmentID", messageAttachmentID);
            var attachment = await _context.Set<MessageAttachment>().FromSqlRaw("exec Pro_Get_Attachment @MessageAttachmentID", attachemntId).ToListAsync();
            return attachment;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
        return new List<MessageAttachment>();

    }
    public async Task<List<MessageDetails>> GetReplies(int parentID, string source = "WEB")
    {

        try
        {

            var pParentID = new SqlParameter("@ParentID", parentID);
            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var pUserID = new SqlParameter("@UserID", CurrentUserID);
            var pSource = new SqlParameter("@Source", source);

            var result = _context.Set<MessageDetails>().FromSqlRaw("exec Pro_User_Message_Reply @ParentID, @CompanyID, @UserID, @Source",
                pParentID, pCompanyID, pUserID, pSource).AsEnumerable().ToList();

            var replies = result.Select(c =>
            {
                c.SentBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                c.AckOptions = (from AK in _context.Set<ActiveMessageResponse>().AsEnumerable()
                                where AK.MessageId == c.MessageId

                                orderby AK.ResponseCode
                                select new AckOption { ResponseId = AK.ResponseId, ResponseLabel = AK.ResponseLabel }).ToList();
                return c;
            }).ToList();

            return replies;


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<MessageGroupObject>> GetMessageGroupList(int messageID)
    {
        try
        {

            var pMessageID = new SqlParameter("@MessageID", messageID);
            var pCompanyID = new SqlParameter("@CompanyID", CurrentCompanyID);
            var pUserID = new SqlParameter("@UserID", CurrentUserID);

            var result = await _context.Set<MessageGroupObject>().FromSqlRaw("Exec Pro_Get_Message_User_Groups @MessageID, @CompanyID, @UserID",
                pMessageID, pCompanyID, pUserID).ToListAsync();

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
            return new List<MessageGroupObject>();
        }
    }

    public async Task<dynamic> ConverToMp3()
    {
        return false;

        //try
        //{
        //    HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    string filePath = string.Empty;
        //    string fileName = string.Empty;
        //    var httpRequest = HttpContext.Current.Request;
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            fileName = postedFile.FileName;
        //            filePath = ServerUploadFolder + postedFile.FileName;
        //            postedFile.SaveAs(filePath);
        //        }
        //    }
        //    else
        //    {
        //        result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    string outfile = AudioConversion.ToMp3(filePath, null);
        //    if (outfile != null)
        //    {

        //        var stream = new MemoryStream(File.ReadAllBytes(outfile), 0, File.ReadAllBytes(outfile).Length, true, true);

        //        HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
        //        httpResponseMessage.Content = new StreamContent(stream);
        //        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
        //        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mp3");

        //        return httpResponseMessage;
        //    }

        //    return result;
        //}
        //catch (Exception ex)
        //{
        //    return null;
        //}
    }

    public object GetConfRecordings(int confCallId, int objectId, string objectType, bool single, int companyId)
    {
        CommonDTO ResultDTO = new CommonDTO();

        try
        {
            objectType = objectType.ToUpper();
            if (confCallId <= 0)
            {
                var recordings = (from CH in _context.Set<ConferenceCallLogHeader>()
                                  join U in _context.Set<User>() on CH.InitiatedBy equals U.UserId
                                  where CH.CompanyId == CurrentCompanyID && CH.TargetObjectId == objectId && CH.TargetObjectName == objectType
                                  select new
                                  {
                                      CH,
                                      U.FirstName,
                                      U.LastName,
                                      ConferenceStart = CH.ConfrenceStart != null ? CH.ConfrenceStart : CH.CreatedOn,
                                      ConferenceEnd = CH.ConfrenceEnd != null ? CH.ConfrenceEnd : CH.CreatedOn
                                  }).ToList();

                return recordings;
            }
            else if (confCallId > 0 && single == true)
            {
                var recording = (from CH in _context.Set<ConferenceCallLogHeader>()
                                 where CH.ConferenceCallId == confCallId
                                 select CH).FirstOrDefault();
                if (recording != null)
                {
                    if (recording.RecordingSid != null)
                    {

                        _DBC.connectUNCPath();

                        string RecordingPath = _DBC.LookupWithKey("RECORDINGS_PATH");
                        string SavePath = @RecordingPath + "\\" + recording.CompanyId + "\\";

                        if (!File.Exists(@SavePath + recording.RecordingSid + ".mp3"))
                        {
                            DownloadRecording(recording.RecordingSid, recording.CompanyId, recording.RecordingUrl);
                        }
                    }
                }
                return recording;
            }
            else if (confCallId > 0 && single == false)
            {
                var recordings = (from CD in _context.Set<ConferenceCallLogDetail>()
                                  join CH in _context.Set<ConferenceCallLogHeader>() on CD.ConferenceCallId equals CH.ConferenceCallId
                                  join UD in _context.Set<User>() on CD.UserId equals UD.UserId
                                  where CD.ConferenceCallId == confCallId
                                  orderby CD.UserId
                                  select new
                                  {
                                      CD,
                                      CD.UserId,
                                      UD.FirstName,
                                      UD.LastName,
                                      UD.Isdcode,
                                      UD.MobileNo,
                                      CH.ConfrenceEnd,
                                      CH.ConfrenceStart
                                  }).ToList().OrderBy(o => o.CD.UserId);

                return recordings;
            }
            return ResultDTO;
        }
        catch (Exception ex)
        {
            return ResultDTO;
        }
    }

    public void DownloadRecording(string recordingSid, int companyId, string recordingUrl)
    {
        try
        {
            int RetryCount = 2;
            string RecordingPath = _DBC.LookupWithKey("RECORDINGS_PATH");
            int.TryParse(_DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT"), out RetryCount);
            string SavePath = @RecordingPath + "\\" + companyId + "\\";

            _DBC.connectUNCPath();

            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

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
                        Client.DownloadFile(recordingUrl, @SavePath + recordingSid + ".mp3");
                        confdownloaded = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        confdownloaded = true;
                    }
                }
                if (confdownloaded)
                {
                    if (File.Exists(@SavePath + recordingSid + ".mp3"))
                    {
                        _CH.DeleteRecording(recordingSid);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        catch (WebException ex)
        {
        }
    }

    public async Task<dynamic> GetConfUser(int objectId, string objectType)
    {
        CommonDTO ResultDTO = new CommonDTO();
        try
        {
            if (objectType.ToUpper() == "INCIDENT")
            {
                var rcpnt_list = await  (from ML in _context.Set<MessageList>()
                                  join M in _context.Set<Message>() on ML.MessageId equals M.MessageId
                                  join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                                  where M.IncidentActivationId == objectId && U.Status == 1 && M.MessageType == "Incident"
                                  select new { U.FirstName, U.LastName, U.UserPhoto, U.Isdcode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToListAsync();

                return rcpnt_list;
            }
            else if (objectType.ToUpper() == "PING")
            {
                var rcpnt_list = await (from ML in _context.Set<MessageList>()
                                  join M in _context.Set<Message>() on ML.MessageId equals M.MessageId
                                  join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                                  where M.MessageId == objectId && U.Status == 1 && M.MessageType == "Ping"
                                  select new { U.FirstName, U.LastName, U.UserPhoto, U.Isdcode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToListAsync();
                return rcpnt_list;
            }
            return ResultDTO;
        }
        catch (Exception ex)
        {
            return ResultDTO;
        }

    }
    /* -- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Confidence Selomo>
-- Create date: <16/09/2022,,>
-- Description:	<SP to Ping Notification List,,>
-- =============================================
CREATE PROCEDURE Pro_Get_Ping_Notification_List 
	-- Add the parameters for the stored procedure here
	@CompanyID int, 
	@MessageID int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		SELECT
		ObjectMappingId = INL.ObjectMappingId,
		SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
		ObjectLabel = L.Location_Name,
		ObjectType ='LOCATION'
		from  AdhocMessageNotificationList INL
		INNER JOIN ObjectRelation ORL on  (INL.ObjectMappingId = ORL.ObjectMappingId)
		AND
		 (INL.SourceObjectPrimaryId= ORL.SourceObjectPrimaryId )
		INNER JOIN ObjectMapping OP on ORL.ObjectMappingId=OP.ObjectMappingId
		Inner JOIN Objects O on  OP.TargetObjectID=O.ObjectID
		INNER JOIN [dbo].[Location] L on INL.SourceObjectPrimaryId = L.LocationId
		WHERE INL.CompanyId = 7 
		AND ORL.ObjectMappingId = 9 
		AND INL.MessageId = @MessageID
		UNION
		SELECT                                                              
		ObjectMappingId = INL.ObjectMappingId,
		SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
		ObjectLabel = D.GroupName,
		ObjectType = 'GROUP'
		FROM  AdhocMessageNotificationList INL
		INNER JOIN  ObjectRelation ORL on  (INL.ObjectMappingId = ORL.ObjectMappingId)
		AND (INL.SourceObjectPrimaryId= ORL.SourceObjectPrimaryId )
		INNER JOIN [dbo].[Group] D on INL.SourceObjectPrimaryId = D.GroupId
		Where INL.CompanyId = @CompanyID AND ORL.ObjectMappingId = 10 AND
		INL.MessageId = @MessageID
                                                               
                                                              
END
GO
     * 
     */

    public async Task<PingInfoReturn> GetPingInfo(int messageId, int userId, int companyId)
    {
        try
        {
            var pingifo = await  _context.Set<Message>()
                                 .Where(M=> M.MessageId == messageId)
                                 .Select(M=> new PingInfoReturn
                                 {
                                     MessageText = M.MessageText,
                                     AssetId = M.AssetId,
                                     CascadePlanID = M.CascadePlanId,
                                     MultiResponse = M.MultiResponse,
                                     Priority = M.Priority,
                                     SilentMessage = M.SilentMessage,
                                     TrackUser = M.TrackUser,
                                     AttachmentCount = (int)M.AttachmentCount,
                                 }).FirstOrDefaultAsync();
            if (pingifo != null)
            {
                var pCompanyId = new SqlParameter("@CompanyID", companyId);
                var pMessageId = new SqlParameter("@MessageID", messageId);
                pingifo.PingNotificationList = await _context.Set<IIncNotificationLst>().FromSqlRaw("Exec Pro_Get_Ping_Notification_List @CompanyID,@MessageID", pCompanyId, pMessageId) .ToListAsync();
                pingifo.AckOptions = await _context.Set<ActiveMessageResponse>()
                                            .Where(MM=> MM.MessageId == messageId)
                                            .Select(MM => new AckOption
                                            {
                                                ResponseId = MM.ResponseId,
                                                ResponseLabel = MM.ResponseLabel,
                                                ResponseCode = MM.ResponseCode
                                            }).ToListAsync();
                pingifo.MessageMethod = await  _context.Set<MessageMethod>().Include(MT=>MT.CommsMethod)                                               
                                               .Where(MM=> MM.MessageId == messageId)
                                               .Select(MM=> new CommsMethods 
                                               { 
                                                   MethodId = MM.MethodId, 
                                                   MethodName = MM.CommsMethod.MethodName 
                                               }).ToListAsync();
                pingifo.UsersToNotify = await  _context.Set<UsersToNotify>().Include(U=>U.User)
                                               .Where(UN=> UN.MessageId == messageId)
                                               .Select(U=> new IIncKeyConResponse
                                               {
                                                   FirstName = U.User.FirstName,
                                                   LastName = U.User.LastName,
                                                   UserId = U.UserId
                                               }).ToListAsync();
                pingifo.DepartmentToNotify = await  _context.Set<AdhocMessageNotificationList>().Include(d => d.Department)                                                    
                                                    .Where(INL=> INL.CompanyId == CurrentCompanyID && INL.ObjectMappingId == 100 &&
                                                    INL.MessageId == messageId)
                                                    .Select(INL=> new IIncNotificationLst
                                                    {
                                                        ObjectMappingId = 100,
                                                        SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
                                                        ObjectLabel = INL.Department.DepartmentName,
                                                        ObjectType = "DEPARTMENT"
                                                    }).ToListAsync();

                return pingifo;
            }
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<PublicAlertRtn>> GetPublicAlert(int companyId, int targetUserId)
    {
        try
        {
            return _PH.GetPublicAlert(companyId, targetUserId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public dynamic GetPublicAlertTemplate(int templateId, int userId, int companyId)
    {
        try
        {
            return _PH.GetPublicAlertTemplate(templateId, userId, companyId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<int> PingMessages(PingMessageQuery pingMessage)
    {
        try
        {
            return await _PH.PingMessages(pingMessage.CompanyId, pingMessage.MessageText, pingMessage.AckOptions,
                pingMessage.Priority, pingMessage.MultiResponse, pingMessage.MessageType, pingMessage.IncidentActivationId,
                pingMessage.CurrentUserId, pingMessage.TimeZoneId, pingMessage.PingMessageObjLst, pingMessage.UsersToNotify,
                pingMessage.AssetId, pingMessage.SilentMessage, pingMessage.MessageMethod, pingMessage.MediaAttachments,
                pingMessage.SocialHandle, pingMessage.CascadePlanID);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public dynamic ProcessPAFile(string userListFile, bool hasHeader, int emailColIndex, int phoneColIndex, int postcodeColIndex, int latColIndex, int longColIndex, string sessionId)
    {
        try
        {
            return _PH.ProcessPAFile(userListFile, hasHeader, emailColIndex, phoneColIndex, postcodeColIndex, latColIndex, longColIndex, sessionId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<CommonDTO> ResendFailure(int messageId, string commsMethod)
    {
        try
        {
            return await _PH.ResendFailure(messageId, commsMethod);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<dynamic> ReplyToMessage(int parentId, string messageText, string replyTo, string messageType, int activeIncidentId, int[] messageMethod, int cascadePlanId, int currentUserId, int companyId, string timeZoneId)
    {
        try
        {
            return await _PH.ReplyToMessage(parentId, messageText, replyTo, messageType, activeIncidentId, messageMethod, cascadePlanId, currentUserId, companyId, timeZoneId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<int> SaveMessageResponse(int responseId, string responseLabel, string description, bool isSafetyResponse,
                     string safetyAckAction, string messageType, int status, int currentUserId, int companyId, string timeZoneId)
    {
        return await _MSG.SaveMessageResponse(responseId, responseLabel, description, isSafetyResponse,
            safetyAckAction, messageType, status, currentUserId, companyId, timeZoneId);
    }

    public async Task<dynamic> SendPublicAlert(string messageText, int[] messageMethod, bool schedulePA, DateTime scheduleAt, string sessionId, int userId, int companyId, string timeZoneId)
    {
        try
        {
            return await _PH.SendPublicAlert(messageText, messageMethod, schedulePA, scheduleAt, sessionId, userId, companyId, timeZoneId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> StartConference(List<User> userList, int objectID, int currentUserID, int companyID, string timeZoneId)
    {
        try
        {
            return await _MSG.StartConference(userList, objectID, currentUserID, companyID, timeZoneId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Return UploadAttachment()
    {
        string ServerUploadFolder = "C:\\Temp\\";
        int iUploadedCnt = 0;
        string FileName = string.Empty;
        try
        {

            string AttachmentSavePath = _DBC.LookupWithKey("ATTACHMENT_SAVE_PATH");
            string AttachmentUncUser = _DBC.LookupWithKey("ATTACHMENT_UNC_USER");
            string AttachmentUncPwd = _DBC.LookupWithKey("ATTACHMENT_UNC_PWD");
            string AttachmentUseUnc = _DBC.LookupWithKey("ATTACHMENT_USE_UNC");
            ServerUploadFolder = _DBC.LookupWithKey("UPLOAD_PATH");

            _DBC.connectUNCPath(AttachmentSavePath, AttachmentUncUser, AttachmentUncPwd, AttachmentUseUnc);

            List<IFormFile> hfc = new List<IFormFile>();

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                //List<IFormFile> hpf = hfc[iCnt];
                //if (hpf.ContentLength > 0)
                //{
                //    string FileExt = System.IO.Path.GetExtension(hpf.FileName);
                //    FileName = Guid.NewGuid().ToString().Replace("-", "") + FileExt;
                //    if (!System.IO.File.Exists(ServerUploadFolder + FileName))
                //    {
                //        hpf.SaveAs(ServerUploadFolder + FileName);
                //        iUploadedCnt = iUploadedCnt + 1;
                //    }
                //}
            }
            if (iUploadedCnt > 0)
            {
                return _DBC.Return(0, "0", true, "File Uploaded successfully", FileName);
            }
            else
            {
                return _DBC.Return(1, "E001", false, "Could not upload file", null);
            }
        }
        catch (Exception ex)
        {
            return _DBC.Return(1, "E001", false, ex.ToString(), null);
        }
    }

}