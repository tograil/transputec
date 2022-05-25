using CrisesControl.Core.Paging;

namespace CrisesControl.Api.Application.Commands.Common;

public class BaseResponse
{
    public object Data { get; set; }
    public string ErrorCode { get; set; }
    public string Message { get; set; }
}