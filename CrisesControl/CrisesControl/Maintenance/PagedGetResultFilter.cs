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
            if (_paging.Apply 
                && context.Result is OkObjectResult { Value: IEnumerable<dynamic> result } okResult
                && _paging.PageSize > 0)
            {
                var resultToReturn = result.ToArray();

                var pagedResult = new PagedResult
                {
                    ListToShow = resultToReturn.Skip(_paging.PageNumber - 1).Take(_paging.PageSize),
                    PageSize = _paging.PageSize,
                    PageIndex = _paging.PageNumber,
                    TotalPages = resultToReturn.Length / _paging.PageSize + ((resultToReturn.Length % _paging.PageSize) > 0 ? 1 : 0),
                    TotalItems = resultToReturn.Length
                };

                okResult.Value = pagedResult;
            }

            await next();
        }
    }
}