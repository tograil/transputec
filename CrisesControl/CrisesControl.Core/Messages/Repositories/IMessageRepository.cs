using System.Threading.Tasks;

namespace CrisesControl.Core.Messages.Repositories;

public interface IMessageRepository
{
    Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0);
}