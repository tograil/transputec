using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents.SP_Response;

public class IncidentAssetResponse : IncidentAssets
{
    public string AssetOwnerFirstName
    {
        get
        {
            return AssetOwnerName?.Firstname ?? null;
        }
        set
        {
            if (AssetOwnerName == null)
            {
                AssetOwnerName = new Users.UserFullName();
            }
            if (AssetOwnerName.Firstname != value)
            {
                AssetOwnerName.Firstname = value;
            }
        }
    }
    public string AssetOwnerLastName
    {
        get
        {
            return AssetOwnerName?.Lastname ?? null;
        }
        set
        {
            if (AssetOwnerName == null)
            {
                AssetOwnerName = new Users.UserFullName();
            }
            if (AssetOwnerName.Lastname != value)
            {
                AssetOwnerName.Lastname = value;
            }
        }
    }
    public string FilePath { get; set; }
}