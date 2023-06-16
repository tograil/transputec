//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IPatchOperation2Base
    {
        OperationName Name { get; set; }
        IPath Path { get; set; }
    }
}
