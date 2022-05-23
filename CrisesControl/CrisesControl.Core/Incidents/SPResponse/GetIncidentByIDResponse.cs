namespace CrisesControl.Core.Incidents.SPResponse;

public class GetIncidentByIDResponse : IncidentDetails
{
    public string CreatedByFirstName
    {
        get
        {
            return CreatedByName?.Firstname ?? null;
        }
        set
        {
            if (CreatedByName == null)
            {
                CreatedByName = new Users.UserFullName();
            }
            if (CreatedByName.Firstname != value)
            {
                CreatedByName.Firstname = value;
            }
        }
    }
    public string CreatedByLastName
    {
        get
        {
            return CreatedByName?.Lastname ?? null;
        }
        set
        {
            if (CreatedByName == null)
            {
                CreatedByName = new Users.UserFullName();
            }
            if (CreatedByName.Lastname != value)
            {
                CreatedByName.Lastname = value;
            }
        }
    }
    public string UpdatedByFirstName
    {
        get
        {
            return UpdatedByName?.Firstname ?? null;
        }
        set
        {
            if (UpdatedByName == null)
            {
                UpdatedByName = new Users.UserFullName();
            }
            if (UpdatedByName.Firstname != value)
            {
                UpdatedByName.Firstname = value;
            }
        }
    }
    public string UpdatedByLastName
    {
        get
        {
            return UpdatedByName?.Lastname ?? null;
        }
        set
        {
            if (UpdatedByName == null)
            {
                UpdatedByName = new Users.UserFullName();
            }
            if (UpdatedByName.Lastname != value)
            {
                UpdatedByName.Lastname = value;
            }
        }
    }
}