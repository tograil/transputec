// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public interface IResourceQuery
    {
        IReadOnlyCollection<string> Attributes { get; }
        IReadOnlyCollection<string> ExcludedAttributes { get; }
        IReadOnlyCollection<IFilter> Filters { get; }
        IPaginationParameters PaginationParameters { get; }
    }
}
