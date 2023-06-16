// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace CC.Authority.SCIM.Service.Monitor
{
    public interface INotification<TPayload>
    {
        long? Identifier { get; }
        string CorrelationIdentifier { get; }
        TPayload Message { get; }
    }
}
