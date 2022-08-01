using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using MediatR;
using Serilog;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageAttachment
{
    public class GetMessageAttachmentHandler : IRequestHandler<GetMessageAttachmentRequest, GetMessageAttachmentResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMessageAttachmentHandler> _logger;
        public GetMessageAttachmentHandler(IMapper mapper, ILogger<GetMessageAttachmentHandler> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        public async Task<GetMessageAttachmentResponse> Handle(GetMessageAttachmentRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var msgResponse = await _messageRepository.GetMessageAttachment(request.MessageListID, request.MessageID);
                var response = _mapper.Map<List<MessageAttachment>>(msgResponse);
                if (response != null)
                {
                    return new GetMessageAttachmentResponse
                    {
                        Data = response,
                        ErrorCode = HttpStatusCode.OK,
                        Message = "Data has been loaded Succesfully"
                    };
                }
                return new GetMessageAttachmentResponse
                {
                    Data = response,
                    ErrorCode = HttpStatusCode.OK,
                    Message = "Data Not Found"
                };



            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }
            
        }
    }
}
