//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class TypedItem
    {
        [DataMember(Name = AttributeNames.Type)]
        public string ItemType
        {
            get;
            set;
        }

        [DataMember(Name = AttributeNames.Primary, IsRequired = false)]
        public bool Primary
        {
            get;
            set;
        }
    }
}