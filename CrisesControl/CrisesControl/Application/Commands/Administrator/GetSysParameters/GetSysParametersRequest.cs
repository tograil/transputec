using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetSysParameters
{
    public class GetSysParametersRequest:IRequest<GetSysParametersResponse>
    {
        public int SystemParameterId { get; set; }
    }
}
