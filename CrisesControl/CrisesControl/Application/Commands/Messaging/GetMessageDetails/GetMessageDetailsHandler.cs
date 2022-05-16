using AutoMapper;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails
{
    public class GetMessageDetailsHandler: IRequestHandler<GetMessageDetailsRequest, GetMessageDetailsResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMessageDetailsHandler> _logger;
        public GetMessageDetailsHandler(IMapper mapper,ILogger<GetMessageDetailsHandler> logger, IMessageRepository messageRepository)
        {
              _logger= logger;
             _messageRepository= messageRepository;
            _mapper= mapper;
        }
        
     public async Task<GetMessageDetailsResponse> Handle(GetMessageDetailsRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var msgResponse = await _messageRepository.GetMessageDetails(request.CloudMsgId, request.MessageId);
                var response = _mapper.Map<UserMessageList>(msgResponse);
                if(msgResponse != null) { 
                return new GetMessageDetailsResponse {
                    Data = response,
                    ErrorCode = System.Net.HttpStatusCode.OK,
                    Message="Data has been loaded Succesfully"
                    };
                }
                return new GetMessageDetailsResponse
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
