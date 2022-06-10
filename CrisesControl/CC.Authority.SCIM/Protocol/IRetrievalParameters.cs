//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IRetrievalParameters
    {
        IReadOnlyCollection<string> ExcludedAttributePaths { get; }
        string Path { get; }
        IReadOnlyCollection<string> RequestedAttributePaths { get; }
        string SchemaIdentifier { get; }
    }
}