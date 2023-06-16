//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IPath
    {
        string AttributePath { get; }
        string SchemaIdentifier { get; }
        IReadOnlyCollection<IFilter> SubAttributes { get; }
        IPath ValuePath { get; }
    }
}