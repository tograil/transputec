using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetSysParameters
{
    public class GetSysParametersResponse
    {
        public List<SysParameter> Data { get; set; }
        public string Message { get; set; }
    }
}
