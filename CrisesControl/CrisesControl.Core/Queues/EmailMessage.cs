namespace CrisesControl.Core.Queues;

public class EmailMessage : MessageQueueItem
{
    public string TwitterPage { get; set; }
    public string TwitterIcon { get; set; }
    public string FacebookPage { get; set; }
    public string FacebookIcon { get; set; }
    public string LinkedInPage { get; set; }
    public string LinkedInIcon { get; set; }
    public string SupportEmail { get; set; }
    public string CCLogo { get; set; }
    public string PortalURL { get; set; }
    public string ACKUrl { get; set; }
    public string TemplatePath { get; set; }
    public string OneClickAcknowledge { get; set; }
    public string Domain { get; set; }
    public string EmailFrom { get; set; }
    public string SMTPHost { get; set; }
    public string EmailSub { get; set; }
    public string SendGridAPIKey { get; set; }
    public string AwsSesAccessKey { get; set; }
    public string AWSSESSecretKey { get; set; }
    public string EmailSecretKey { get; set; }
    public string EmailProvider { get; set; }
    public string Office365Host { get; set; }

}