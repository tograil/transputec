//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public interface IBulkCreationOperationState : IBulkOperationState<Resource>
    {
        IReadOnlyCollection<IBulkUpdateOperationContext> Dependents { get; }
        IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates { get; }

        void AddDependent(IBulkUpdateOperationContext dependent);
        void AddSubordinate(IBulkUpdateOperationContext subordinate);
    }
}
