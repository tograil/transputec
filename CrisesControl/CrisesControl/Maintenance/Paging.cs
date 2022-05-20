﻿using CrisesControl.Api.Maintenance.Interfaces;

namespace CrisesControl.Api.Maintenance
{
    public class Paging : IPaging
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public bool Apply { get; set; }
    }
}