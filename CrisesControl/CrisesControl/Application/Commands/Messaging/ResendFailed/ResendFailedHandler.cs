using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ResendFailed
{
    public class ResendFailedHandler : IRequestHandler<ResendFailedRequest, ResendFailedResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public ResendFailedHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<ResendFailedResponse> Handle(ResendFailedRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ResendFailedRequest));
            var response = new ResendFailedResponse();
            var common = await _messageRepository.ResendFailure(request.messageId, request.commsMethod);
            var result =  _mapper.Map<CommonDTO>(common);
            response.CommonDTO = result;
            return response;
        }
    }
}
