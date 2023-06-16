﻿using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetIcons
{
    public class GetIconsResponse
    {
        public List<Icon> Data { get; set; }
        public string Message { get; set; }
    }
}
