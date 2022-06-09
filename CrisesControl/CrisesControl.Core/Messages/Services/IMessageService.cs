using System.Threading.Tasks;

namespace CrisesControl.Core.Messages.Services;

public interface IMessageService
{
    Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false);
    Task CreateMessageMethod(int MessageID, int MethodID, int ActiveIncidentID = 0, int IncidentID = 0);


}