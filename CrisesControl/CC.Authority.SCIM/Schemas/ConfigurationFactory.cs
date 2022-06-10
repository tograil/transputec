//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    public abstract class ConfigurationFactory<TConfiguration, TException>
        where TException : Exception
    {
        public abstract TConfiguration Create(
            Lazy<TConfiguration> defaultConfiguration,
            out TException configurationError);
    }
}