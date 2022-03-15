using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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
}