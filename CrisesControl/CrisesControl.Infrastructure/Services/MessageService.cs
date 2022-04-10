using System.Threading.Tasks;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Messages.Services;

namespace CrisesControl.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false)
    {
        throw new System.NotImplementedException();
    }
}