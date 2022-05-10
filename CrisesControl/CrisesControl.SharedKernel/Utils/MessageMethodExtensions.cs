using System;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.SharedKernel.Utils;

public static class MessageMethodExtensions
{
    public static string ToDbString(this MessageMethod messageMethod)
    {
        return messageMethod switch
        {
            MessageMethod.Email => "EMAIL",
            MessageMethod.Push => "PUSH",
            MessageMethod.Phone => "PHONE",
            MessageMethod.Text => "TEXT",
            MessageMethod.All => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(messageMethod), messageMethod, null)
        };
    }
}