//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    internal class BulkOperationState<TPayload> : BulkOperationStateBase<TPayload> where TPayload : class
    {
        public BulkOperationState(
            IRequest<BulkRequest2> request,
            BulkRequestOperation operation,
            IBulkOperationContext<TPayload> context)
            : base(request, operation, context)
        {
        }

        public override bool TryPrepareRequest(out IRequest<TPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
