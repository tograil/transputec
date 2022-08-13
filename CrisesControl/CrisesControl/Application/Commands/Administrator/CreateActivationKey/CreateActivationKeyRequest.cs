using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.CreateActivationKey
{
    public class CreateActivationKeyRequest:IRequest<CreateActivationKeyResponse>
    {
        public int CustomerId { get; set; }
        public int SalesSource { get; set; }
    }
}
