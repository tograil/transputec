using MediatR;

namespace CrisesControl.Api.Application.Commands.App.ValidatePin
{
    public class ValidatePinRequest:IRequest<ValidatePinResponse>
    {
        public int PinNumber { get; set; }
    }
}
