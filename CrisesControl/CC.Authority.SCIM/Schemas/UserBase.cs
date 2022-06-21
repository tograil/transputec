//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class UserBase : Resource
    {
        [DataMember(Name = AttributeNames.UserName)]
        public virtual string UserName
        {
            get;
            set;
        }
    }
}