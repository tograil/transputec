using System;
using System.Text;

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
    public static string Base64Decode(string base64EncodedData)
    {
       
            byte[] base64EncodedBytes = Encoding.ASCII.GetBytes(base64EncodedData);
            string bytes = BitConverter.ToString(base64EncodedBytes);
            return bytes;
        
    }
}