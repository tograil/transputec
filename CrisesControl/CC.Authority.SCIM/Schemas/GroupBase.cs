//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class GroupBase : Resource
    {
        [DataMember(Name = AttributeNames.DisplayName)]
        public virtual string DisplayName
        {
            get;
            set;
        }

        [DataMember(Name = AttributeNames.Members, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Member> Members
        {
            get;
            set;
        }
    }
}