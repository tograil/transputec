using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetAttachment
{
    public class GetAttachmentValidator: AbstractValidator<GetAttachmentRequest> {
       public GetAttachmentValidator()
        {
       
            RuleFor(x => x.MessageAttachmentID).GreaterThan(0);
        }
      }
}
