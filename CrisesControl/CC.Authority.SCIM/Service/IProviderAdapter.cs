// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public interface IProviderAdapter<T> where T : Resource
    {
        string SchemaIdentifier { get; }

        Task<Resource> Create(HttpRequestMessage request, Resource resource, string correlationIdentifier);
        Task Delete(HttpRequestMessage request, string identifier, string correlationIdentifier);
        Task<QueryResponseBase> Query(
            HttpRequestMessage request,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters,
            string correlationIdentifier);
        Task<Resource> Replace(HttpRequestMessage request, Resource resource, string correlationIdentifier);
        Task<Resource> Retrieve(
            HttpRequestMessage request,
            string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            string correlationIdentifier);
        Task Update(
            HttpRequestMessage request,
            string identifier,
            PatchRequestBase patchRequest,
            string correlationIdentifier);
    }
}
