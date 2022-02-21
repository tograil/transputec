using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompleteRegistration;

public class CompleteRegistrationRequest : IRequest<CompleteRegistrationResponse>
{
    public int RegId { get; set; }
}