﻿using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport
{
    public class GetUserIncidentReportResponse
    {
        public UserIncidentReportResponse data { get; set; }
        public string Message { get; set; }
    }
}
