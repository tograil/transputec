using System.Collections.Generic;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Core.Queues;

public class PushMessage : MessageQueueItem
{
    public string AppleCertPwd { get; set; }
    public string AppleCertPath { get; set; }
    public bool ApplePushMode { get; set; }
    public string GoogleApiKey { get; set; }
    public string SoundFileName { get; set; }
    public string BBApplicationID { get; set; }
    public string BBPassword { get; set; }
    public string BBPushURL { get; set; }
    public bool UseCopilot { get; set; }
    public int TrackingDuration { get; set; }
    public string WinAppSID { get; set; }
    public string WinClientSecret { get; set; }
    public string WinPackageName { get; set; }
    public string WinDeskSID { get; set; }
    public string WinDeskClientSecret { get; set; }
    public string WinDeskPackageName { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public string PortalUrl { get; set; }
}