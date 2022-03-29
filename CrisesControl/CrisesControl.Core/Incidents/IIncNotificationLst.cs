namespace CrisesControl.Core.Incidents;

public class IIncNotificationLst
{
    public int ObjectMappingId { get; set; }
    public int SourceObjectPrimaryId { get; set; }
    public string ObjectType { get; set; }
    public string ObjectLabel { get; set; }
}