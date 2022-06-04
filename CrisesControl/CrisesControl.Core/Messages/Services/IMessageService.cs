using System.Threading.Tasks;

namespace CrisesControl.Core.Messages.Services;

public interface IMessageService
{
    Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false);
    
}