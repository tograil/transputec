using System.Collections.Generic;

namespace CrisesControl.SharedKernel.Utils;

public class Roles
{
    public static string[] CcRoles(bool addKeyHolder = false, bool addUser = false)
    {
        var roleList = new List<string> { "ADMIN", "SUPERADMIN" };
        if (addKeyHolder)
            roleList.Add("KEYHOLDER");

        if (addUser)
            roleList.Add("USER");

        return roleList.ToArray();
    }
}