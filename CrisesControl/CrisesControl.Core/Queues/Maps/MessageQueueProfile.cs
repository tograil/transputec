using AutoMapper;

namespace CrisesControl.Core.Queues.Maps;

public class MessageQueueProfile : Profile
{
    public MessageQueueProfile()
    {
        CreateMap<MessageQueueItem, EmailMessage>();

        CreateMap<MessageQueueItem, PushMessage>();
    }
}