// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace CC.Authority.SCIM.Service.Monitor
{
    public interface IExceptionNotification : INotification<Exception>
    {
        bool Critical { get; set; }
    }
}
