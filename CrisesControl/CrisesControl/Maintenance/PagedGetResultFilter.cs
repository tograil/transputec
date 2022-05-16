using CrisesControl.Api.Maintenance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace CrisesControl.Api.Maintenance
{
    public class PagedGetResultFilter : IAsyncResultFilter
    {
        private readonly IPaging _paging;

        public PagedGetResultFilter(IPaging paging)
        {
            _paging = paging;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_paging.Apply && context.Result is OkObjectResult { Value: IEnumerable<dynamic> result } okResult )
            {
                okResult.Value = result.Skip(_paging.PageNumber - 1).Take(_paging.PageSize);
            }

            await next();
        }
    }
}