//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class InstantMessagingBase : TypedValue
    {
        public const string Aim = "aim";
        public const string Gtalk = "gtalk";
        public const string Icq = "icq";
        public const string Msn = "msn";
        public const string Qq = "qq";
        public const string Skype = "skype";
        public const string Xmpp = "xmpp";
        public const string Yahoo = "yahoo";

        internal InstantMessagingBase()
        {
        }
    }
}