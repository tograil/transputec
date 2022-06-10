// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace CC.Authority.SCIM.Service.Monitor
{
    public interface IInformationNotification : INotification<string>
    {
        bool Verbose { get; set; }
    }
}
