using CrisesControl.Core.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents;

public class IncidentParticipants
{
    public int IncidentParticipantID { get; set; }
    public int IncidentID { get; set; }
    public int IncidentActionID { get; set; }
    public string GroupType { get; set; }
    public string GroupName { get; set; }
    public string ParticipantType { get; set; }
    public int ParticipantGroupID { get; set; }
    public int ObjectMappingID { get; set; }
    public int ParticipantUserID { get; set; }
    [NotMapped]
    public UserFullName UserName
    {
        get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}