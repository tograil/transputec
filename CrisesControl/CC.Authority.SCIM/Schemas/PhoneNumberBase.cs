//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class PhoneNumberBase : TypedValue
    {
        public const string Fax = "fax";
        public const string Home = "home";
        public const string Mobile = "mobile";
        public const string Other = "other";
        public const string Pager = "pager";
        public const string Work = "work";

        internal PhoneNumberBase()
        {
        }
    }
}
