//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public interface IBulkCreationOperationContext : IBulkOperationContext<Resource>, IBulkCreationOperationState
    {
        IBulkOperationState<Resource> PendingState { get; }
    }
}
