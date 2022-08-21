using CrisesControl.Core.Companies;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveScimProfile
{
    public class SaveScimProfileRequest:IRequest<SaveScimProfileResponse>
    {
        public CompanyScim Model { get; set; }
    }
}
