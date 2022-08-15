using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddSysParameters
{
    public class AddSysParametersRequest:IRequest<AddSysParametersResponse>
    {
      
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
