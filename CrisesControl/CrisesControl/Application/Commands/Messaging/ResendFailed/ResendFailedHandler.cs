using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ResendFailed
{
    public class ResendFailedHandler : IRequestHandler<ResendFailedRequest, ResendFailedResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public ResendFailedHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ResendFailedResponse> Handle(ResendFailedRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ResendFailedRequest));
            var response = new ResendFailedResponse();
            return response;
        }
    }
}
