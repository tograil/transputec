using System;

namespace CrisesControl.SharedKernel.Utils;

public static class Common
{
    public static int GetPriority(int severity)
    {
        var prio = severity switch
        {
            >= 0 and <= 2 => 100,
            > 2 and <= 4 => 500,
            5 => 999,
            _ => 100
        };

        return Convert.ToInt32(prio);
    }
}