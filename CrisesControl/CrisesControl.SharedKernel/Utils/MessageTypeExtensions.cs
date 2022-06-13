﻿using System;
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
    public static string ToDbKeyString(this KeyType keyType)
    {
        return keyType switch
        {
            KeyType.ALLOWCHANGECHANNELUSER => "ALLOW_CHANGE_CHANNEL_USER",
            KeyType.ALLOWCHANNELPRIORITY => "ALLOW_CHANNEL_PRIORITY",
            KeyType.ALLOWCHANGEPRIORITYUSER => "ALLOW_CHANGE_PRIORITY_USER",
            KeyType.PHONEVALIDATIONMETHOD=> "PHONE_VALIDATION_METHOD",
            KeyType.TWILIOUSEINDIRECTCONNECTION=> "TWILIO_USE_INDIRECT_CONNECTION",
            KeyType.TWILIOROUTINGAPI=> "TWILIO_ROUTING_API",
            KeyType.PHONEVALIDATIONMSG=> "PHONE_VALIDATION_MSG",
            KeyType.TWILIOFROMNUMBER=> "TWILIO_FROM_NUMBER",
            KeyType.MESSAGINGCOPILOTSID=> "MESSAGING_COPILOT_SID",
            KeyType.USEMESSAGINGCOPILOT => "USE_MESSAGING_COPILOT",
            KeyType.MESSAGERETRYCOUNT=> "_MESSAGE_RETRY_COUNT",
            KeyType.APICLASS=> "_API_CLASS",
            KeyType._CLIENTID=> "_CLIENTID",
            KeyType.CLIENTSECRET=> "_CLIENT_SECRET",
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };
    }
    public static string ToDbMethodString(this MethodType methodType)
    {
        return methodType switch
        {
            MethodType.Ping => "PING",
            MethodType.Incident => "INCIDENT",
            
            _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
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