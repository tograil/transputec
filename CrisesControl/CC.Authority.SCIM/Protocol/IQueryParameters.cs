//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IQueryParameters : IRetrievalParameters
    {
        IReadOnlyCollection<IFilter> AlternateFilters { get; }
        IPaginationParameters PaginationParameters { get; set; }
    }
}