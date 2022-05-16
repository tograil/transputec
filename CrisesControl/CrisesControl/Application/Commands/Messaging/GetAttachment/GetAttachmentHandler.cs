using AutoMapper;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Messaging.GetAttachment
{
    public class GetAttachmentHandler: IRequestHandler<GetAttachmentRequest, GetAttachmentResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAttachmentHandler> _logger;
        public GetAttachmentHandler(IMapper mapper,ILogger<GetAttachmentHandler> logger, IMessageRepository messageRepository)
        {
              _logger= logger;
             _messageRepository= messageRepository;
            _mapper= mapper;
        }
        
     public async Task<GetAttachmentResponse> Handle(GetAttachmentRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var msgResponse = await _messageRepository.GetAttachment( request.MessageAttachmentID);
                var response = _mapper.Map< List<MessageAttachment>>(msgResponse);
                if(response != null) { 
                       return new GetAttachmentResponse
                          {
                            Data = response,
                            ErrorCode = System.Net.HttpStatusCode.OK,
                            Message="Data has been loaded Succesfully"
                          };
                }
                return new GetAttachmentResponse
                {
                    Data = response,
                    ErrorCode = System.Net.HttpStatusCode.OK,
                    Message = "Data Not Found"
                };


            } 
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;

            }
        }
    }
}
