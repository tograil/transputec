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

namespace CrisesControl.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{

    private int UserID;
    private int CompanyID;
    private readonly string TimeZoneId = "GMT Standard Time";
    private readonly string ServerUploadFolder = "C:\\Temp\\";

    private readonly CrisesControlContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MessageRepository> _logger;
    private readonly ICompanyParametersRepository _companyParameters;
    private readonly DBCommon _DBC;
    private readonly CommsHelper _CH;


    public MessageRepository(
        CrisesControlContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MessageRepository> logger,
        ICompanyParametersRepository companyParameters,
        DBCommon DBC,
        CommsHelper CH
        )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _companyParameters = companyParameters;
        _DBC = DBC;
        _CH = CH;
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
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
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

            UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
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

    public async Task CopyMessageResponse(int CompanyID, int CurrentUserId, string TimeZoneID, CancellationToken token)
    {
        try
        {
            var rsps = await GetLibMessageResponse();
            foreach (var rsp in rsps)
            {
                int responseid = await CreateMessageResponse(rsp.ResponseLabel, (bool)rsp.IsSafetyOption, rsp.MessageType,
                    "NONE", rsp.Status, CurrentUserId, CompanyID, TimeZoneID, token);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<int> CreateMessageResponse(string ResponseLabel, bool SOSEvent, string MessageType, string SafetyAction, int Status,
            int CurrentUserId, int CompanyID, string TimeZoneId, CancellationToken token)
    {
        try
        {

            CompanyMessageResponse msgResponse = new CompanyMessageResponse();
            msgResponse.ResponseLabel = ResponseLabel;
            msgResponse.Description = ResponseLabel;
            msgResponse.IsSafetyResponse = SOSEvent;
            msgResponse.SafetyAckAction = SafetyAction;
            msgResponse.MessageType = MessageType;
            msgResponse.Status = Status;
            msgResponse.UpdatedBy = CurrentUserId;
            msgResponse.CompanyId = CompanyID;
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

            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
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

    public async Task<AcknowledgeReturn> AcknowledgeMessage(int UserID, int MessageID, int MessageListID, string Latitude, string Longitude, string AckMethod, int ResponseID, string TimeZoneId)
    {
        try
        {
            if (string.IsNullOrEmpty(Longitude))
            {

            }
            DateTimeOffset dtNow = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pMessageID = new SqlParameter("@MessageID", MessageID);
            var pMessageListID = new SqlParameter("@MessageListID", MessageListID);
            var pLatitude = new SqlParameter("@Latitude", Latitude ?? string.Empty);
            var pLongitude = new SqlParameter("@Longitude", Longitude ?? string.Empty);
            var pMode = new SqlParameter("@Mode", AckMethod ?? string.Empty);
            var pTimestamp = new SqlParameter("@Timestamp", dtNow);
            var pResponseID = new SqlParameter("@ResponseID", ResponseID);


            var MessageData = _context.Set<AcknowledgeReturn>().FromSqlRaw("exec Pro_Message_Acknowledge @UserID,@MessageID,@MessageListID, @Latitude, @Longitude,@Mode,@Timestamp,@ResponseID",
                                             pUserID, pMessageID, pMessageListID, pLatitude, pLongitude, pMode, pTimestamp, pResponseID).AsEnumerable<AcknowledgeReturn>();
            if (MessageData != null)
            {
                var message = MessageData.FirstOrDefault();
                return message;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);


        }
        return new AcknowledgeReturn { };
    }

    public async Task<MessageAckDetails> MessageAcknowledged(int CompanyId, int MsgListId, string TimeZoneId, string UserLocationLat, string UserLocationLong, int CurrentUserId, int ResponseID = 0, string AckMethod = "WEB")
    {
        try
        {
            const string message = "Acknowleged";
            AcknowledgeReturn MessageData = await AcknowledgeMessage(CurrentUserId, 0, MsgListId, UserLocationLat, UserLocationLong, AckMethod, ResponseID, TimeZoneId);

            if (MessageData != null)
            {

                if (ResponseID > 0)
                {
                    int CallbackOption = await GetCallbackOption(AckMethod);
                    CheckSOSAlert(MsgListId, "ACKNOWLEGE", CallbackOption);
                }

                if (AckMethod != "APP")
                {
                    AddUserTrackingDevices(CurrentUserId);
                }

                var list = await _context.Set<MessageList>().Where(MsgList => MsgList.MessageListId == MsgListId).Select(n =>
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
                var msg = (await _context.Set<MessageList>().Where(MsgList => MsgList.MessageListId == MsgListId).Select(x =>
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

    public async Task<int> GetCallbackOption(string AckMethod)
    {
        int CallbackOption = 3;
        if (AckMethod == MessageType.Email.ToDbString())
        {
            CallbackOption = 1;
        }
        else if (AckMethod == MessageType.Text.ToString())
        {
            CallbackOption = 1;
        }
        return CallbackOption;
    }
    public async void AddUserTrackingDevices(int UserID, int MessageListID = 0)
    {

        var devices = await _context.Set<UserDevice>().Where(UD => UD.UserId == UserID).ToListAsync();
        foreach (var device in devices)
        {
            if (device.DeviceType.ToUpper().Contains(Enum.GetName(typeof(DeviceType), DeviceType.ANDROID)) || device.DeviceType.ToUpper().Contains(Enum.GetName(typeof(DeviceType), DeviceType.WINDOWS)))
                AddTrackingDevice(device.CompanyId, device.UserDeviceId, device.DeviceId, device.DeviceType, MessageListID);
        }
    }
    public async void AddTrackingDevice(int CompanyID, int UserDeviceID, string DeviceAddress, string DeviceType, int MessageListID = 0)
    {
        const string messageText = "Track Device";
        try
        {
            MessageDevice MessageDev = new MessageDevice();
            MessageDev.CompanyId = CompanyID;
            MessageDev.MessageId = 0;
            MessageDev.MessageListId = MessageListID;
            MessageDev.UserDeviceId = UserDeviceID;
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
            MessageDev.DeviceAddress = DeviceAddress;
            MessageDev.DeviceType = DeviceType;
            await _context.AddAsync(MessageDev);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                     ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
    }
    public async void CheckSOSAlert(int MessageListID, string SOSType, int CallbackOption)
    {
        try
        {
            const string key = "SOS_ENABLED";
            var sos = await _context.Set<MessageList>().Include(M => M.Message).Include(AMR => AMR.ActiveMessageResponse).Where(ML => ML.MessageListId == MessageListID).FirstOrDefaultAsync();

            if (sos != null)
            {
                bool IsSOSEnabled = false;
                bool.TryParse(await _companyParameters.GetCompanyParameter(key, sos.Message.CompanyId), out IsSOSEnabled);

                if (sos.ActiveMessageResponse.IsSafetyResponse && IsSOSEnabled)
                {

                    var check = await _context.Set<Sosalert>().Where(SA => SA.UserId == sos.RecepientUserId && (SA.ActiveIncidentId == sos.Message.IncidentActivationId &&
         SA.ActiveIncidentId != 0 && sos.Message.IncidentActivationId != 0)).FirstOrDefaultAsync();
                    if (check != null)
                    {
                        check.ActiveIncidentId = sos.Message.IncidentActivationId;
                        check.AlertType = SOSType ?? string.Empty;
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
                        CreateSOSAlert(sos.RecepientUserId, SOSType, sos.Message.MessageId, sos.MessageListId, sos.ResponseId, (int)sos.Message.IncidentActivationId,
                            sos.ActiveMessageResponse.ResponseLabel ?? string.Empty, sos.UpdatedOn, DateTime.Now, sos.UserLocationLat ?? string.Empty, sos.UserLocationLong ?? String.Empty, CallbackOption);
                    }
                }
            }
        }
        catch (Exception)
        {

        }
    }
    public async void CreateSOSAlert(int UserID, string SOSType, int MessageId, int MessageListId, int ResponseID, int IncidentActivationId,
            string ResponseLabel, DateTimeOffset UpdatedOn, DateTimeOffset ResponseTimeGMT, string Lat, string Lng, int CallbackOption)
    {

        try
        {
            Sosalert SA = new Sosalert();
            SA.UserId = UserID;
            SA.ActiveIncidentId = IncidentActivationId;
            SA.AlertType = SOSType;
            SA.Latitude = Lat.Left(15);
            SA.Longitude = Lng.Left(15);
            SA.MessageId = MessageId;
            SA.MessageListId = MessageListId;
            SA.ResponseId = ResponseID;
            SA.ResponseLabel = ResponseLabel;
            SA.ResponseTime = UpdatedOn;
            SA.ResponseTimeGmt = ResponseTimeGMT;
            SA.CallbackOption = CallbackOption;
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



    public async Task<List<NotificationDetails>> MessageNotifications(int CompanyId, int CurrentUserId)
    {
        List<NotificationDetails> NDL = new List<NotificationDetails>();

        List<IIncidentMessages> IM = await _get_incident_message(CompanyId, CurrentUserId);
        List<IPingMessage> PM = await _get_ping_message(CompanyId, CurrentUserId);

        NDL.Add(new NotificationDetails { IncidentMessages = IM, PingMessage = PM });
        return NDL;
    }
    public async Task<string> LookupWithKey(string Key, string Default = "")
    {
        try
        {
            Dictionary<string, string> Globals = CCConstants.GlobalVars;
            if (Globals.ContainsKey(Key))
            {
                return Globals[Key];
            }


            var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
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

    public async Task<List<IIncidentMessages>> _get_incident_message(int CompanyId, int CurrentUserId)
    {
        var pTargetUserId = new SqlParameter("@UserID", CurrentUserId);
        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);

        var result = await _context.Set<IIncidentMessages>().FromSqlRaw(" exec Pro_User_Incident_Notifications @UserID, @CompanyID",
            pTargetUserId, pCompanyID).ToListAsync();

        result.Select(async c =>
        {
            c.AckOptions = new List<AckOption>(await _context.Set<ActiveMessageResponse>().Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true).Select(n =>
                   new AckOption()
                   {
                       ResponseId = n.ResponseId,
                       ResponseLabel = n.ResponseLabel,
                   }).ToListAsync());
            return c;
        }).ToList();
        return result;





    }

    public async Task<List<IPingMessage>> _get_ping_message(int CompanyId, int CurrentUserId)
    {

        var pTargetUserId = new SqlParameter("@UserID", CurrentUserId);
        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);

        var result = await _context.Set<IPingMessage>().FromSqlRaw("exec Pro_User_Ping_Notifications @UserID, @CompanyID",
            pTargetUserId, pCompanyID).ToListAsync();

        result.Select(async c =>
        {
            c.AckOptions = new List<AckOption>(await _context.Set<ActiveMessageResponse>().Where(AK => AK.MessageId == c.MessageId && c.MultiResponse == true).Select(n =>
                  new AckOption()
                  {
                      ResponseId = n.ResponseId,
                      ResponseLabel = n.ResponseLabel,
                  }).ToListAsync());
            return c;
        }).ToList();
        return result;

    }

    public async Task<IncidentMessageDetails> GetMessageDetails(string CloudMsgId, int MessageId = 0)
    {
        CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
        var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
        var pUserId = new SqlParameter("@UserID", UserID);

        //int MsgListid = Base64Decode(CloudMsgId);
        var MsgListid = await _context.Set<MessageDevice>().FirstOrDefaultAsync(ml => ml.CloudMessageId == CloudMsgId);
        try
        {
            if (MessageId > 0)
            {
                var msglistid = await _context.Set<MessageList>().FirstOrDefaultAsync(ml => ml.MessageListId == MsgListid.MessageListId);
                if (msglistid != null)
                {
                    var newmsglistid = await _context.Set<MessageList>().FirstOrDefaultAsync(ML => ML.MessageId == MessageId && ML.RecepientUserId == msglistid.RecepientUserId && ML.MessageAckStatus == 0);

                    if (newmsglistid != null)
                    {
                        MsgListid.MessageListId = newmsglistid.MessageListId;
                    }

                    var CheckType = await _context.Set<MessageList>().Include(ML => ML.Message).FirstOrDefaultAsync(ML => ML.MessageListId == MsgListid.MessageListId && ML.MessageId == ML.Message.MessageId);

                    //Read sql parameters

                    var ActiveIncidentID = new SqlParameter("@ActiveIncidentID", CheckType.Message.IncidentActivationId);
                    var RecepientUserId = new SqlParameter("@UserID", CheckType.RecepientUserId);
                    var CompanyId = new SqlParameter("@CompanyID", CheckType.Message.CompanyId);
                    var messageId = new SqlParameter("@MessageID", CheckType.MessageId);


                    if (CheckType != null)
                    {
                        if (CheckType.Message.MessageType == Enum.GetName(typeof(MessageCheckType), MessageCheckType.Incident))
                        {
                            var IncidentMessageDetails = await _context.Set<IncidentMessageDetails>().FromSqlRaw("exec Pro_Get_Incident_Message_List @ActiveIncidentID,@UserID, @CompanyID", ActiveIncidentID, RecepientUserId, CompanyId).FirstOrDefaultAsync();
                            return IncidentMessageDetails;
                        }
                        else if (CheckType.Message.MessageType == Enum.GetName(typeof(MessageCheckType), MessageCheckType.Ping))
                        {
                            var PingMessageDetails = await _context.Set<IncidentMessageDetails>().FromSqlRaw("exec Pro_Get_Ping_Message_List @UserID,@CompanyID, @MessageID, @IncidentActivationID", RecepientUserId, CompanyId, messageId, ActiveIncidentID).FirstOrDefaultAsync();
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

    public async Task<List<MessageAttachment>> GetMessageAttachment(int MessageListID, int MessageID)
    {
        try
        {
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var messageListId = new SqlParameter("@MessageListID", MessageListID);
            var messageId = new SqlParameter("@MessageID", MessageID);
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

    public async Task<List<MessageAttachment>> GetAttachment(int MessageAttachmentID)
    {

        try
        {
            var attachemntId = new SqlParameter("@MessageAttachmentID", MessageAttachmentID);
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
    public async Task<List<MessageDetails>> GetReplies(int ParentID, string Source = "WEB")
    {

        try
        {
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            var pParentID = new SqlParameter("@ParentID", ParentID);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pSource = new SqlParameter("@Source", Source);

            var result = _context.Set<MessageDetails>().FromSqlRaw("Pro_User_Message_Reply @ParentID, @CompanyID, @UserID, @Source",
                pParentID, pCompanyID, pUserID, pSource).AsEnumerable();

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

    public async Task<List<MessageGroupObject>> GetMessageGroupList(int MessageID)
    {
        try
        {

            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            var pMessageID = new SqlParameter("@MessageID", MessageID);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);

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
                                  where CH.CompanyId == CompanyID && CH.TargetObjectId == objectId && CH.TargetObjectName == objectType
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

    public object GetConfUser(int objectId, string objectType)
    {
        CommonDTO ResultDTO = new CommonDTO();
        try
        {
            if (objectType.ToUpper() == "INCIDENT")
            {
                var rcpnt_list = (from ML in _context.Set<MessageList>()
                                  join M in _context.Set<Message>() on ML.MessageId equals M.MessageId
                                  join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                                  where M.IncidentActivationId == objectId && U.Status == 1 && M.MessageType == "Incident"
                                  select new { U.FirstName, U.LastName, U.UserPhoto, U.Isdcode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToList();

                return rcpnt_list;
            }
            else if (objectType.ToUpper() == "PING")
            {
                var rcpnt_list = (from ML in _context.Set<MessageList>()
                                  join M in _context.Set<Message>() on ML.MessageId equals M.MessageId
                                  join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                                  where M.MessageId == objectId && U.Status == 1 && M.MessageType == "Ping"
                                  select new { U.FirstName, U.LastName, U.UserPhoto, U.Isdcode, U.MobileNo, U.PrimaryEmail, U.UserId }).Distinct().ToList();
                return rcpnt_list;
            }
            return ResultDTO;
        }
        catch (Exception ex)
        {
            return ResultDTO;
        }

    }

    public async Task<PingInfoReturn> GetPingInfo(int messageId, int userId, int companyId)
    {
        try
        {
            var pingifo = await (from M in _context.Set<Message>()
                                 where M.MessageId == messageId
                                 select new PingInfoReturn
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
                pingifo.PingNotificationList = await (from INL in _context.Set<AdhocMessageNotificationList>()
                                                      join ORL in _context.Set<ObjectRelation>() on new { INL.ObjectMappingId, INL.SourceObjectPrimaryId }
                                                      equals new { ORL.ObjectMappingId, ORL.SourceObjectPrimaryId }
                                                      join L in _context.Set<Location>() on INL.SourceObjectPrimaryId equals L.LocationId
                                                      where INL.CompanyId == CompanyID && ORL.ObjectMappingId == 9 &&
                                                      INL.MessageId == messageId
                                                      select new IIncNotificationLst
                                                      {
                                                          ObjectMappingId = INL.ObjectMappingId,
                                                          SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
                                                          ObjectLabel = L.LocationName,
                                                          ObjectType = "LOCATION"
                                                      }).Union((from INL in _context.Set<AdhocMessageNotificationList>()
                                                                join ORL in _context.Set<ObjectRelation>() on new { INL.ObjectMappingId, INL.SourceObjectPrimaryId }
                                                                equals new { ORL.ObjectMappingId, ORL.SourceObjectPrimaryId }
                                                                join D in _context.Set<Group>() on INL.SourceObjectPrimaryId equals D.GroupId
                                                                where INL.CompanyId == CompanyID && ORL.ObjectMappingId == 10 &&
                                                                INL.MessageId == messageId
                                                                select new IIncNotificationLst
                                                                {
                                                                    ObjectMappingId = INL.ObjectMappingId,
                                                                    SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
                                                                    ObjectLabel = D.GroupName,
                                                                    ObjectType = "GROUP"
                                                                })
                               ).ToListAsync();
                pingifo.AckOptions = await (from MM in _context.Set<ActiveMessageResponse>()
                                            where MM.MessageId == messageId
                                            select new AckOption
                                            {
                                                ResponseId = MM.ResponseId,
                                                ResponseLabel = MM.ResponseLabel,
                                                ResponseCode = MM.ResponseCode
                                            }).ToListAsync();
                pingifo.MessageMethod = await (from MM in _context.Set<MessageMethod>()
                                               join MT in _context.Set<CommsMethod>() on MM.MethodId equals MT.CommsMethodId
                                               where MM.MessageId == messageId
                                               select new CommsMethods { MethodId = MM.MethodId, MethodName = MT.MethodName }).ToListAsync();
                pingifo.UsersToNotify = await (from UN in _context.Set<UsersToNotify>()
                                               join U in _context.Set<User>() on UN.UserId equals U.UserId
                                               where UN.MessageId == messageId
                                               select new IIncKeyConResponse
                                               {
                                                   FirstName = U.FirstName,
                                                   LastName = U.LastName,
                                                   UserId = U.UserId
                                               }).ToListAsync();
                pingifo.DepartmentToNotify = await (from INL in _context.Set<AdhocMessageNotificationList>()
                                                    join D in _context.Set<Department>() on INL.SourceObjectPrimaryId equals D.DepartmentId
                                                    where INL.CompanyId == CompanyID && INL.ObjectMappingId == 100 &&
                                                    INL.MessageId == messageId
                                                    select new IIncNotificationLst
                                                    {
                                                        ObjectMappingId = 100,
                                                        SourceObjectPrimaryId = INL.SourceObjectPrimaryId,
                                                        ObjectLabel = D.DepartmentName,
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

}