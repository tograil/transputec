using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration
{
    public class DeleteTempRegistrationRequest:IRequest<DeleteTempRegistrationResponse>
    {
        public string UniqueReference { get; set; }
    }
}
