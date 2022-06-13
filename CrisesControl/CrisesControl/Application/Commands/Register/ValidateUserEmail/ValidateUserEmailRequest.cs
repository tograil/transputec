using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Register.ValidateUserEmail
{
    public class ValidateUserEmailRequest :IRequest<ValidateUserEmailResponse>
    {
        
        public int CompanyId { get; set; }
       
        public string uniqueId { get; set; }
    }
}
