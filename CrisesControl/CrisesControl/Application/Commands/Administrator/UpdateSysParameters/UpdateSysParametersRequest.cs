using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateSysParameters
{
    public class UpdateSysParametersRequest:IRequest<UpdateSysParametersResponse>
    {
        public int SysParametersId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
