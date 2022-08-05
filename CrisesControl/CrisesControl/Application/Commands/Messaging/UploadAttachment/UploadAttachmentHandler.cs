using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.UploadAttachment
{
    public class UploadAttachmentHandler : IRequestHandler<UploadAttachmentRequest, UploadAttachmentResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public UploadAttachmentHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<UploadAttachmentResponse> Handle(UploadAttachmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UploadAttachmentRequest));
            var response = new UploadAttachmentResponse();
            return response;
        }
    }
}
