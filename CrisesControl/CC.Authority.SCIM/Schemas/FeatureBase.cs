//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public abstract class FeatureBase
    {
        [DataMember(Name = AttributeNames.Supported)]
        public bool Supported
        {
            get;
            set;
        }
    }
}