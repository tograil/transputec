using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Register.SendVerification
{
    public class SendVerificationRequest : IRequest<SendVerificationResponse>
    {
        [MaxLength(50)]
        public string UniqueId { get; set; }
    }
}
