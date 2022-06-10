//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Service
{
    public interface IBulkUpdateOperationState : IBulkOperationState<IPatch>
    {
        IReadOnlyCollection<IBulkCreationOperationContext> Dependencies { get; }
        IBulkCreationOperationContext Parent { get; }

        void AddDependency(IBulkCreationOperationContext dependency);
    }
}
