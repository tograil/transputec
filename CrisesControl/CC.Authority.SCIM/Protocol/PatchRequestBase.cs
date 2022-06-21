//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Protocol
{
    [DataContract]
    public abstract class PatchRequestBase : Schematized
    {
    }
}