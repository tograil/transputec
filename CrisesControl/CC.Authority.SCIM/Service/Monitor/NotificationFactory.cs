// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace CC.Authority.SCIM.Service.Monitor
{
    public abstract class NotificationFactory<TPayload, TNotification>
    {
        public abstract TNotification CreateNotification(
            TPayload payload,
            string correlationIdentifier,
            long? identifier);

        public TNotification CreateNotification(TPayload payload, Guid correlationIdentifier, long? identifier)
        {
            string correlationIdentifierValue = correlationIdentifier.ToString();
            TNotification result = this.CreateNotification(payload, correlationIdentifierValue, identifier);
            return result;
        }
    }
}
