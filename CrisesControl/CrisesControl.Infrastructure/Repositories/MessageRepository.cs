using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
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
}