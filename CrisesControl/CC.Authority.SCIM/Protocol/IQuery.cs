//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IQuery
    {
        IReadOnlyCollection<IFilter> AlternateFilters { get; set; }
        IReadOnlyCollection<string> ExcludedAttributePaths { get; set; }
        IPaginationParameters PaginationParameters { get; set; }
        string Path { get; set; }
        IReadOnlyCollection<string> RequestedAttributePaths { get; set; }

        string Compose();
    }
}