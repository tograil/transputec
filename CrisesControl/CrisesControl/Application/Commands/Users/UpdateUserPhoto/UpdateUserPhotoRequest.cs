using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhoto
{
    public class UpdateUserPhotoRequest : IRequest<UpdateUserPhotoResponse>
    {
        public string UserPhoto { get; set; }
    }
}
