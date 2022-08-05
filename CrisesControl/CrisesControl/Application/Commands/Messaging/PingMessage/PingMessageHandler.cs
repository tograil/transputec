using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageHandler:IRequestHandler<PingMessageRequest, PingMessageResponse>
    {
        private readonly IMapper _mapper;
        public PingMessageHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<PingMessageResponse> Handle(PingMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(PingMessageRequest));
            var response = new PingMessageResponse();
            return response;
        }
    }
}
