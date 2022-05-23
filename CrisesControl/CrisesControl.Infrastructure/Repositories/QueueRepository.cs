using System.Threading.Tasks;
using CrisesControl.Core.Queues.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Repositories;

public class QueueRepository : IQueueRepository
{
    private readonly CrisesControlContext _context;

    public QueueRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task CreateMessageQueue(int messageId, MessageType messageType, int priority, int cascadePlanId = 0)
    {
        var pMessageId = new SqlParameter("@MessageId", messageId);
        var pMessageType = new SqlParameter("MessageType", messageType.ToDbString());
        var pPriority = new SqlParameter("@Priority", priority);

        var spName = cascadePlanId > 0 ? "Pro_Create_Message_Queue_Cascading" : "Pro_Create_Message_Queue";

        await _context.Database.ExecuteSqlRawAsync($"{spName} {pMessageId}, {pMessageType}, {pPriority}");
    }
}