using System.Net;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Exceptions.Error.Base;
using CrisesControl.Core.Exceptions.NotFound.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrisesControl.Api.Maintenance
{
    public class ErrorFilter : IExceptionFilter
    {
        private readonly ILogger<ErrorFilter> _logger;
        private readonly ICurrentUser _currentUser;

        public ErrorFilter(ILogger<ErrorFilter> logger,
            ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Error in controller happened");

            //TODO: add option for stack trace in config

            var result = context.Exception switch
            {
                NotFoundBaseException notFoundBaseException => new JsonResult(new ErrorData
                {
                    CompanyId = notFoundBaseException.ErrorData.CompanyId,
                    UserId = notFoundBaseException.ErrorData.UserId,
                    Message = notFoundBaseException.Message,
                    StackTrace = notFoundBaseException.StackTrace!,
                })
                {
                    StatusCode = StatusCodes.Status404NotFound
                },
                ErrorBaseException errorBaseException => new JsonResult(new ErrorData
                {
                    CompanyId = errorBaseException.ErrorData.CompanyId,
                    UserId = errorBaseException.ErrorData.UserId,
                    Message = errorBaseException.Message,
                    StackTrace = errorBaseException.StackTrace!,
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                },
                { } exception => new JsonResult(new ErrorData
                {
                    Message = exception.Message,
                    StackTrace = exception.StackTrace!,
                    UserId = _currentUser.UserId,
                    CompanyId = _currentUser.CompanyId
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };

            context.Result = result;
        }
    }
}