using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrisesControl.Api.Maintenance
{
    public class ErrorFilter : IExceptionFilter
    {
        private readonly ILogger<ErrorFilter> _logger;

        public ErrorFilter(ILogger<ErrorFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Error in controller happened");

            var result = new ErrorData
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace!
            };

            context.Result = new JsonResult(result);
        }
    }
}