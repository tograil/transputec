using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.StartConference
{
    public class StartConferenceHandler : IRequestHandler<StartConferenceRequest, StartConferenceResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public StartConferenceHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<StartConferenceResponse> Handle(StartConferenceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(StartConferenceRequest));
            var response = new StartConferenceResponse();
            response.Status = await _messageRepository.StartConference(request.UserList, request.ObjectID, request.CurrentUserID, request.CompanyID, request.TimeZoneId);
            return response;
        }
    }
}
