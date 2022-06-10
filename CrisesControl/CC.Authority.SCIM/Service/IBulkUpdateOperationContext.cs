//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Service
{
    public interface IBulkUpdateOperationContext : IBulkOperationContext<IPatch>, IBulkUpdateOperationState
    {
    }
}
