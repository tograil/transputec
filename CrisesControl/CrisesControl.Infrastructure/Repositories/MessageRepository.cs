using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using MessageMethod = CrisesControl.Core.Messages.MessageMethod;

namespace CrisesControl.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly CrisesControlContext _context;

    public MessageRepository(CrisesControlContext context)
    {
        _context = context;
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

    public async Task<int> CreateMessage(int companyId, string msgText, string messageType, int incidentActivationId, int priority,
        int currentUserId, int source, DateTimeOffset localTime, bool multiResponse, ICollection<AckOption> ackOptions, int status = 0,
        int assetId = 0, int activeIncidentTaskId = 0, bool trackUser = false, bool silentMessage = false,
        int[] messageMethod = null, ICollection<MediaAttachment> mediaAttachments = null, int parentId = 0, int messageActionType = 0)
    {
        if (parentId > 0 && incidentActivationId == 0 && messageType.ToUpper() == "INCIDENT")
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
                    CreateIncidentNotificationList(messageId, incidentActivationId,
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
}