using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetConfRecordings
{
    public class GetConfRecordingsHandler:IRequestHandler<GetConfRecordingsRequest, GetConfRecordingsResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public GetConfRecordingsHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<GetConfRecordingsResponse> Handle(GetConfRecordingsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetConfRecordingsRequest));
            var result = _messageRepository.GetConfRecordings(request.ConfCallId, request.ObjectID, request.ObjectType, request.Single, request.OutUserCompanyId);
            return _mapper.Map<GetConfRecordingsResponse>(result);
        }
    }
}
