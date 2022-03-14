using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.CreateLocation
{
    public class CreateLocationRequest : IRequest<CreateLocationResponse>
    {
        public int GroupId { get; set; }  
        public int CompanyId { get; set; }
        public string? GroupName { get; set; }
    }
}
