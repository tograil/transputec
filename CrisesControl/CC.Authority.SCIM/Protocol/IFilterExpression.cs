//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    internal interface IFilterExpression
    {
        IReadOnlyCollection<IFilter> ToFilters();
    }
}