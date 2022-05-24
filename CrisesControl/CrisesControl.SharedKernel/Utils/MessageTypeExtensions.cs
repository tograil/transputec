using System;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.SharedKernel.Utils;

public static class MessageTypeExtensions
{
    public static string ToDbString(this MessageType messageType)
    {
        return messageType switch
        {
            MessageType.Email => "EMAIL",
            MessageType.Phone => "PHONE",
            MessageType.Push => "PUSH",
            MessageType.Text => "TEXT",
            _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
        };
    }
    public static string ToMemString(this MemberShipType membershipType)
    {
        return membershipType switch
        {
            MemberShipType.NON_MEMBER => "Non-Member",
            MemberShipType.MEMBER => "Member",
            
            _ => throw new ArgumentOutOfRangeException(nameof(membershipType), membershipType, null)
        };
    }
}