using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempDept
{
    public class GetTempDeptRequest:IRequest<GetTempDeptResponse>
    {
        public int TempDeptId { get; set; }
    }
}
