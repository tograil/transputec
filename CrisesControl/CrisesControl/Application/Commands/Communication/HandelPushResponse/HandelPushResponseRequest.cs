using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelPushResponse
{
    public class HandelPushResponseRequest:IRequest<HandelPushResponse>
    { 

        public int SendBackId { get; set; }
    }
}
