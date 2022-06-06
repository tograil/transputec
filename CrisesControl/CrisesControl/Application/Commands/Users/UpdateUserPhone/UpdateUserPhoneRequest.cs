using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhone
{
    public class UpdateUserPhoneRequest : IRequest<UpdateUserPhoneResponse>
    {
        public string MobileISDCode { get; set; }
        public string MobileNo { get; set; }
    }
}
