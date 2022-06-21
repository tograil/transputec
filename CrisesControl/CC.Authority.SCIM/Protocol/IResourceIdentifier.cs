//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IResourceIdentifier
    {
        string Identifier { get; set; }
        string SchemaIdentifier { get; set; }
    }
}