// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public interface IRequest
    {
        Uri BaseResourceIdentifier { get; }
        string CorrelationIdentifier { get; }
        IReadOnlyCollection<IExtension> Extensions { get; }
        HttpRequestMessage Request { get; }
    }

    public interface IRequest<TPayload> : IRequest where TPayload : class
    {
        TPayload Payload { get; }
    }
}
