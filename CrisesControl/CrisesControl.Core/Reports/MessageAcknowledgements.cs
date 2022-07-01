using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports;

public class MessageAcknowledgements
{
    public DateTimeOffset DateAcknowledge { get; set; }
    public DateTimeOffset MessageSent { get; set; }
    public DateTimeOffset DateSent { get; set; }
    public DateTimeOffset MessageAcknowledge { get; set; }
    public string MessageLat { get; set; }
    public string MessageLng { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }
    public string PrimaryEmail { get; set; }
    public string ISDCode { get; set; }
    public string MobileNo { get; set; }
    public string LLISDCode { get; set; }
    public string Landline { get; set; }
    public int UserId { get; set; }
    public string UserPhoto { get; set; }
    public int MessageAckStatus { get; set; }
    public string AckMethod { get; set; }
    public bool IsTaskRecepient { get; set; }
    public string ResponseLabel { get; set; }
    public bool IsSafetyResponse { get; set; }
    public int ActiveIncidentID { get; set; }
}


