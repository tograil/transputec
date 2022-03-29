using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Core.Messages.Repositories;

public interface IMessageRepository
{
    Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0);

    int GetPushMethodId();

    Task AddUserToNotify(int messageId, ICollection<int> userIds, int activeIncidentId = 0);

    Task SaveActiveMessageResponse(int messageId, ICollection<AckOption> ackOptions, int activeIncidentId = 0);
}