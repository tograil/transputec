using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;

namespace CrisesControl.Core.Compatibility.Jobs;

[Obsolete("Added for compatibility with old portal")]
public class PingMessageModel : CcBase
{
    public PingMessageModel()
    {
        AudioAssetId = 0;
        MessageMethod = null;
        AckOptions = null;
        SilentMessage = false;
        MultiResponse = false;
        CascadePlanID = 0;
    }
    /// <summary>
    /// Pass this variable when sending Incident updates, else pass 0
    /// </summary>
    [Required(ErrorMessage = "Incident activation id is required")]
    public int IncidentActivationId { get; set; }
    /// <summary>
    /// Pass this variable with one of the values Low:100, Medium:500 or High:999
    /// </summary>
    public int Priority { get; set; }
    /// <summary>
    /// Message text to be passed in this variable. Frontend field should be set to 250 chars
    /// </summary>
    public string MessageText { get; set; }
    /// <summary>
    /// Pass the MessageType as "Ping" or "Incident"
    /// </summary>
    [MaxLength(20)]
    public string MessageType { get; set; }
    /// <summary>
    /// Send 0 or 1 (send 0 as default).
    /// </summary>
    public bool MultiResponse { get; set; }
    /// <summary>
    /// Send 0 or 1 (send 0 as default).
    /// </summary>
    public List<AckOption> AckOptions { get; set; }

    //public double AssetSize { get; set; }
    /// <summary>
    /// Asset id of the preuploaded asset as audio file.
    /// </summary>
    public int AudioAssetId { get; set; }
    /// <summary>
    /// List an array of objects of Location to Nofity and Departent to notifiy, click to see the param details
    /// </summary>
    public PingMessageObjLst[] PingMessageObjLst { get; set; }
    /// <summary>
    /// Send a an array of integer UserId
    /// </summary>
    public int[] UsersToNotify { get; set; }
    public bool SilentMessage { get; set; }
    public int[] MessageMethod { get; set; }
    public List<MediaAttachment> MediaAttachments { get; set; }
    public List<string> SocialHandle { get; set; }
    public int CascadePlanID { get; set; }
}

public class PingMessageObjLst
{

    /// <summary>
    /// Send the Object Mapping Id location or department
    /// </summary>
    public int ObjectMappingId { get; set; }
    /// <summary>
    /// Send LocationId or GroupId in this parameter
    /// </summary>
    public int SourceObjectPrimaryId { get; set; }

}