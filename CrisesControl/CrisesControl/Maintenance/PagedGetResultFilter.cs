using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.SharedKernel.Utils;
using Dynamitey;
using Humanizer;
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
                && context.Result is OkObjectResult { Value: IEnumerable<dynamic> result } okResult)
            {
                var enumerable = result as dynamic[] ?? result.ToArray();
                var orderName = _paging.OrderBy.Dehumanize();
                var resultToReturn =
                    orderName == string.Empty 
                                    || !enumerable.Any()
                                     || !NameInDynamicExists.NameExists(enumerable.First(), orderName) 
                        ? enumerable.ToArray()
                        : enumerable.OrderBy(x => Dynamic.InvokeGet(x, orderName)).ToArray();
                okResult.Value = resultToReturn;
                //if (_paging.PageSize > 0)
                //{
                //    var pagedResult = new PagedResult
                //    {
                //        ListToShow = resultToReturn.Skip(_paging.PageNumber - 1).Take(_paging.PageSize),
                //        PageSize = _paging.PageSize,
                //        PageIndex = _paging.PageNumber,
                //        TotalPages = resultToReturn.Length / _paging.PageSize +
                //                     ((resultToReturn.Length % _paging.PageSize) > 0 ? 1 : 0),
                //        TotalItems = resultToReturn.Length
                //    };

                //    okResult.Value = pagedResult;
                //}
                //else
                //{
                //    okResult.Value = resultToReturn;
                //}
            }

            await next();
        }
    }
}