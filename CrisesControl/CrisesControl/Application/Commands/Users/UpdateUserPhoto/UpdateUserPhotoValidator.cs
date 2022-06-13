using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhoto
{
    public class UpdateUserPhotoValidator : AbstractValidator<UpdateUserPhotoRequest>
    {
        public UpdateUserPhotoValidator()
        {
            RuleFor(x => x.UserPhoto)
                .NotEmpty();
        }
    }
}
