namespace CrisesControl.Api.Application.Commands.Common;

public record ResultResponse(int ErrorId, string ErrorCode, string Message);