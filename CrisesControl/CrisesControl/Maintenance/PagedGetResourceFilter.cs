using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.SharedKernel.Constants;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrisesControl.Api.Maintenance
{
    public class PagedGetResourceFilter : IAsyncResourceFilter
    {
        public PagedGetResourceFilter(IPaging paging)
        {
            _paging = paging;
        }

        private readonly IPaging _paging;

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method != HttpMethod.Get.Method)
            {
                _paging.PageNumber = 1;
                _paging.PageSize = 30;
                _paging.Apply = false;

                await next();

                return;
            }

            var pageNumber = context.HttpContext.Request.Query["pageNumber"];
            var pageSize = context.HttpContext.Request.Query["pageSize"];

            if (pageNumber.Any() && int.TryParse(pageNumber.First(), out var iPageNumber) && iPageNumber > 0)
            {
                _paging.PageNumber = iPageNumber;
            }
            else
            {
                _paging.PageNumber = PagingConstants.DefaultPageNumber;
            }

            if (pageSize.Any() && int.TryParse(pageSize.First(), out var iPageSize) && iPageSize >= 0)
            {
                _paging.PageSize = iPageSize;
            }
            else
            {
                _paging.PageSize = PagingConstants.DefaultPageSize;
            }

            _paging.Apply = true;

            await next();
        }
    }
}