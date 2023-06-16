//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface ISchemaIdentifier
    {
        string Value { get; }

        string FindPath();
        bool TryFindPath(out string path);
    }
}