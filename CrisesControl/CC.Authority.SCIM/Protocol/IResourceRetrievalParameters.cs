//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IResourceRetrievalParameters : IRetrievalParameters
    {
        IResourceIdentifier ResourceIdentifier { get; }
    }
}