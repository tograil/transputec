using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.GetTempRegistration
{
    public class GetTempRegistrationRequest:IRequest<GetTempRegistrationReponse>
    {
        public int RegId { get; set; }
        public string UniqueRef { get; set; }
    }
}
