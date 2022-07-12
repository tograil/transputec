﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [DataContract]
    public sealed class Feature : FeatureBase
    {
        public Feature(bool supported)
        {
            this.Supported = supported;
        }
    }
}