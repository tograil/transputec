﻿using CrisesControl.Api.Maintenance.Interfaces;
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
            var pageIndex = context.HttpContext.Request.Query["start"];
            var dir = context.HttpContext.Request.Query["dir"];
            var uniqueKey = context.HttpContext.Request.Query["companyKey"];
            var filters = context.HttpContext.Request.Query["filters"];
            var pageSize = context.HttpContext.Request.Query["length"];
            var orderBy = context.HttpContext.Request.Query["orderBy"];
            var search = context.HttpContext.Request.Query["search"];

            int.TryParse(!string.IsNullOrEmpty(pageIndex) ? pageIndex.First() : "0", out var iPageIndex);
            int.TryParse(!string.IsNullOrEmpty(pageSize) ? pageSize.First() : "500", out var iPageLength);

            _paging.Start = iPageIndex;
            _paging.UniqueKey = !string.IsNullOrEmpty(uniqueKey) ? uniqueKey.First() : "";
            _paging.Filters = !string.IsNullOrEmpty(filters) ? filters.First() : "";
            _paging.Dir = !string.IsNullOrEmpty(dir) ? dir.First() : "asc";
            _paging.Length = iPageLength;
            _paging.Search = !string.IsNullOrEmpty(search) ? search.First() : "";

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

            if (orderBy.Any())
            {
                _paging.OrderBy = orderBy.First();
            }
            else
            {
                _paging.OrderBy = string.Empty;
            }

            _paging.Apply = true;

            await next();
        }
    }
}