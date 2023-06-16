//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class TypedValue : TypedItem
    {
        [DataMember(Name = AttributeNames.Value, Order = 0)]
        public string Value
        {
            get;
            set;
        }
    }
}