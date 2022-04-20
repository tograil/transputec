using AutoMapper;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query {
    public class MessageQuery : IMessageQuery {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageQuery(IMessageRepository messageRepository, IMapper mapper) {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request) {
            var countresult = await _messageRepository.GetNotificationsCount(request.CurrentUserId);
            GetNotificationsCountResponse result = _mapper.Map<UserMessageCount, GetNotificationsCountResponse>(countresult);
            return result;
        }
    }
}
