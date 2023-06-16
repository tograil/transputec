using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ProcessPAFile
{
    public class ProcessPAFileHandler : IRequestHandler<ProcessPAFileRequest, ProcessPAFileResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public ProcessPAFileHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<ProcessPAFileResponse> Handle(ProcessPAFileRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ProcessPAFileRequest));
            var response = new ProcessPAFileResponse();
            response =await _messageRepository.ProcessPAFile(request.UserListFile, request.HasHeader, request.EmailColIndex, request.PhoneColIndex, request.PostcodeColIndex, request.LatColIndex, request.LongColIndex, request.SessionId);
            return response;
        }
    }
}
