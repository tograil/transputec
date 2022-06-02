using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Reports;
public class PingReportGrid
{
    public DateTimeOffset DateSent { get; set; }
    public DateTimeOffset DateAcknowledge { get; set; }
    public string MessageText { get; set; }
    public int UserId { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }
    public UserFullName UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ContactNumber { get; set; }
    public string ContactEmail { get; set; }
    public string AckMethod { get; set; }
    public int MessageAckStatus { get; set; }
    public int MessageCount { get; set; }
    public int MessageListId { get; set; }
}